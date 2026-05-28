import { existsSync, readdirSync, readFileSync } from 'node:fs';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const workspaceRoot = resolve(__dirname, '..');
const lockFilePath = resolve(workspaceRoot, 'pnpm-lock.yaml');

const workspaceRoots = ['apps', 'packages'];
const dependencySections = ['dependencies', 'devDependencies', 'peerDependencies', 'optionalDependencies'];
const ignoredPackages = new Set([]);
const requiredSingleVersionPackages = [
  '@mui/x-data-grid',
  '@mui/x-data-grid-pro',
  '@mui/x-data-grid-premium',
  '@mui/x-date-pickers',
  '@mui/x-date-pickers-pro',
  '@mui/x-license',
];

const isSkippableSpecifier = (value) =>
  value.startsWith('workspace:') ||
  value.startsWith('link:') ||
  value.startsWith('file:') ||
  value.startsWith('portal:') ||
  value.startsWith('patch:') ||
  value.startsWith('github:') ||
  value.startsWith('git+') ||
  value.startsWith('http:') ||
  value.startsWith('https:');

const collectWorkspaceManifests = () => {
  const manifests = [];

  for (const root of workspaceRoots) {
    const rootPath = join(workspaceRoot, root);
    if (!existsSync(rootPath)) continue;

    for (const entry of readdirSync(rootPath)) {
      const manifestPath = join(rootPath, entry, 'package.json');
      if (!existsSync(manifestPath)) continue;

      manifests.push({
        workspaceName: `${root}/${entry}`,
        manifest: JSON.parse(readFileSync(manifestPath, 'utf8')),
      });
    }
  }

  return manifests;
};

const collectDependencySpecs = (manifests) => {
  const packageToSpecs = new Map();

  for (const { workspaceName, manifest } of manifests) {
    for (const section of dependencySections) {
      const dependencies = manifest[section] ?? {};
      for (const [packageName, specifier] of Object.entries(dependencies)) {
        if (ignoredPackages.has(packageName)) continue;
        if (typeof specifier !== 'string' || isSkippableSpecifier(specifier)) continue;

        if (!packageToSpecs.has(packageName)) {
          packageToSpecs.set(packageName, []);
        }

        packageToSpecs.get(packageName).push({ workspaceName, section, specifier });
      }
    }
  }

  return packageToSpecs;
};

const findSpecifierMismatches = (packageToSpecs) => {
  const mismatches = [];

  for (const [packageName, usages] of packageToSpecs) {
    const workspacesUsingPackage = new Set(usages.map((usage) => usage.workspaceName));
    if (workspacesUsingPackage.size < 2) continue;

    const uniqueSpecifiers = new Set(usages.map((usage) => usage.specifier));
    if (uniqueSpecifiers.size <= 1) continue;

    const bySpecifier = new Map();
    for (const usage of usages) {
      if (!bySpecifier.has(usage.specifier)) {
        bySpecifier.set(usage.specifier, []);
      }
      bySpecifier.get(usage.specifier).push(`${usage.workspaceName} (${usage.section})`);
    }

    mismatches.push({ packageName, bySpecifier });
  }

  return mismatches.sort((a, b) => a.packageName.localeCompare(b.packageName));
};

const getLockfileVersions = (lockText, packageName) => {
  const escapedPackage = packageName.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
  const pattern = new RegExp(`^\\s{2}'${escapedPackage}@([^'(:\\n]+)`, 'gm');
  const versions = new Set();

  let match;
  while ((match = pattern.exec(lockText)) !== null) {
    versions.add(match[1]);
  }

  return versions;
};

const findLockfileMismatches = (lockText, packageToSpecs) => {
  const mismatches = [];

  for (const [packageName, usages] of packageToSpecs) {
    const workspacesUsingPackage = new Set(usages.map((usage) => usage.workspaceName));
    if (workspacesUsingPackage.size < 2) continue;

    const versions = getLockfileVersions(lockText, packageName);
    if (versions.size <= 1) continue;

    mismatches.push({ packageName, versions: Array.from(versions).sort() });
  }

  return mismatches.sort((a, b) => a.packageName.localeCompare(b.packageName));
};

const manifests = collectWorkspaceManifests();
const packageToSpecs = collectDependencySpecs(manifests);
const specifierMismatches = findSpecifierMismatches(packageToSpecs);

let hasFailure = false;

if (specifierMismatches.length > 0) {
  hasFailure = true;
  console.error('Found cross-workspace dependency specifier mismatches:');
  for (const mismatch of specifierMismatches) {
    console.error(`- ${mismatch.packageName}`);
    for (const [specifier, usages] of mismatch.bySpecifier) {
      console.error(`  ${specifier}`);
      for (const usage of usages.sort()) {
        console.error(`    - ${usage}`);
      }
    }
  }
}

const lockText = readFileSync(lockFilePath, 'utf8');
const lockfileMismatches = findLockfileMismatches(lockText, packageToSpecs);

if (lockfileMismatches.length > 0) {
  hasFailure = true;
  console.error('Found multi-version lockfile entries for shared workspace dependencies:');
  for (const mismatch of lockfileMismatches) {
    console.error(`- ${mismatch.packageName}: ${mismatch.versions.join(', ')}`);
  }
}

const requiredPackageMismatches = [];
for (const packageName of requiredSingleVersionPackages) {
  const versions = getLockfileVersions(lockText, packageName);
  if (versions.size !== 1) {
    requiredPackageMismatches.push({ packageName, versions: Array.from(versions).sort() });
  }
}

if (requiredPackageMismatches.length > 0) {
  hasFailure = true;
  console.error('Found required packages that are not pinned to exactly one lockfile version:');
  for (const mismatch of requiredPackageMismatches) {
    const versionList = mismatch.versions.length === 0 ? '(none found)' : mismatch.versions.join(', ');
    console.error(`- ${mismatch.packageName}: expected exactly 1 version, found ${mismatch.versions.length} -> ${versionList}`);
  }
}

if (hasFailure) {
  process.exit(1);
}

console.log('All shared dependencies are version-synchronized across apps/* and packages/*, and required packages are pinned to exactly one lockfile version.');
