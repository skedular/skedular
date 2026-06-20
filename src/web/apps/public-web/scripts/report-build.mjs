import { readdir, stat } from "node:fs/promises";
import path from "node:path";

const outputDirectory = path.resolve("dist");

async function collectFiles(directory) {
  const entries = await readdir(directory, { withFileTypes: true });
  const files = [];

  for (const entry of entries) {
    const entryPath = path.join(directory, entry.name);
    if (entry.isDirectory()) {
      files.push(...(await collectFiles(entryPath)));
    } else {
      files.push(entryPath);
    }
  }

  return files;
}

const files = await collectFiles(outputDirectory);
const sizes = await Promise.all(
  files.map(async (file) => (await stat(file)).size),
);
const metadata = {
  event: "public-web.build.complete",
  pageCount: files.filter((file) => file.endsWith(".html")).length,
  outputBytes: sizes.reduce((total, size) => total + size, 0),
};

console.log(JSON.stringify(metadata));
