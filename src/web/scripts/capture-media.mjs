#!/usr/bin/env node
import { spawnSync } from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const scriptDir = path.dirname(fileURLToPath(import.meta.url));
const webRoot = path.resolve(scriptDir, '..');
const appsDir = path.join(webRoot, 'apps');
const validApps = new Set(['webapp', 'webapp-spaces', 'webapp-teams']);

const timestamp = () => new Date().toISOString().replace(/[:.]/g, '-');
const log = (event, context = {}) => console.log(JSON.stringify({ event, timestamp: new Date().toISOString(), ...context }));

const parseArgs = (args) => {
  const [appName, scenario = 'all', ...rest] = args;
  const outputIndex = rest.indexOf('--output');
  const outputDir = outputIndex >= 0 ? rest[outputIndex + 1] : undefined;
  return { appName, scenario, outputDir };
};

const run = (command, args, options = {}) => {
  const result = spawnSync(command, args, {
    stdio: 'inherit',
    shell: false,
    ...options,
  });

  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
};

const collectFiles = (dir, extension) => {
  if (!fs.existsSync(dir)) {
    return [];
  }

  const files = [];
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const entryPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      files.push(...collectFiles(entryPath, extension));
    } else if (entry.name.endsWith(extension)) {
      files.push(entryPath);
    }
  }

  return files;
};

const convertVideosToMp4 = (captureDir) => {
  const ffmpeg = spawnSync('ffmpeg', ['-version'], { stdio: 'ignore' });
  if (ffmpeg.status !== 0) {
    throw new Error('ffmpeg is required to convert Playwright WebM videos to MP4/H.264.');
  }

  const webmFiles = collectFiles(captureDir, '.webm');
  for (const webmFile of webmFiles) {
    const mp4File = webmFile.replace(/\.webm$/, '.mp4');
    run('ffmpeg', ['-y', '-i', webmFile, '-c:v', 'libx264', '-pix_fmt', 'yuv420p', '-vf', 'scale=1920:1080', mp4File]);
    log('ui_test_video_converted', { source: webmFile, target: mp4File, format: 'mp4', codec: 'h264', width: 1920, height: 1080 });
  }

  return webmFiles.map((file) => file.replace(/\.webm$/, '.mp4'));
};

const main = () => {
  const { appName, scenario, outputDir } = parseArgs(process.argv.slice(2));
  if (!validApps.has(appName)) {
    console.error('Usage: pnpm capture:media <webapp|webapp-spaces|webapp-teams> [scenario] [--output ./path]');
    process.exit(1);
  }

  const appPath = path.join(appsDir, appName);
  const mediaRoot = path.resolve(outputDir ?? process.env.VIDEO_OUTPUT_DIR ?? path.join(webRoot, '.test-artifacts', 'media'));
  const captureDir = path.join(mediaRoot, appName, scenario, timestamp());
  const screenshotDir = path.join(captureDir, 'screenshots');
  fs.mkdirSync(captureDir, { recursive: true });
  fs.mkdirSync(screenshotDir, { recursive: true });

  log('ui_test_media_capture_started', { appId: appName, scenario, captureDir, screenshotDir, width: 1920, height: 1080 });
  run('pnpm', ['exec', 'playwright', 'test', 'tests/e2e', `--grep=${scenario === 'all' ? '.*' : scenario}`, '--project=chromium-capture', `--output=${captureDir}`], {
    cwd: appPath,
    env: {
      ...process.env,
      PLAYWRIGHT_RECORD_VIDEO: 'true',
      PLAYWRIGHT_CAPTURE_SCREENSHOTS: 'true',
      VIDEO_OUTPUT_DIR: captureDir,
      SCREENSHOT_OUTPUT_DIR: screenshotDir,
    },
  });

  const mp4Files = convertVideosToMp4(captureDir);
  const pngFiles = collectFiles(screenshotDir, '.png');
  log('ui_test_media_capture_completed', { appId: appName, scenario, captureDir, mp4Count: mp4Files.length, pngCount: pngFiles.length });

  if (mp4Files.length === 0 || pngFiles.length === 0) {
    throw new Error(`Media capture for ${appName}/${scenario} did not produce both MP4 videos and PNG screenshots.`);
  }
};

main();

