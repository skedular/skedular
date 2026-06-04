import { spawn } from "node:child_process";
import { describe, expect, it } from "vitest";

const signupUrl = "https://app.example.test/sign-up?source=public-web";

function runBuild(environment: NodeJS.ProcessEnv) {
  return new Promise<{ code: number | null; stdout: string; stderr: string }>((resolve) => {
    const child = spawn("pnpm", ["build"], {
      cwd: process.cwd(),
      env: environment,
      shell: false,
    });
    let stdout = "";
    let stderr = "";

    child.stdout.on("data", (chunk) => {
      stdout += chunk;
    });
    child.stderr.on("data", (chunk) => {
      stderr += chunk;
    });
    child.on("close", (code) => resolve({ code, stdout, stderr }));
  });
}

describe("public website build diagnostics", () => {
  it("emits structured page count and output size metadata without exposing the CTA URL", async () => {
    const result = await runBuild({ ...process.env, PUBLIC_SKEDULAR_SIGNUP_URL: signupUrl });

    expect(result.code).toBe(0);
    expect(result.stdout).toContain('"event":"public-web.build.complete"');
    expect(result.stdout).toMatch(/"pageCount":\d+/);
    expect(result.stdout).toMatch(/"outputBytes":\d+/);
    expect(`${result.stdout}${result.stderr}`).not.toContain(signupUrl);
  });

  it("fails clearly when the required CTA URL is missing", async () => {
    const environment = { ...process.env };
    delete environment.PUBLIC_SKEDULAR_SIGNUP_URL;

    const result = await runBuild(environment);

    expect(result.code).not.toBe(0);
    expect(`${result.stdout}${result.stderr}`).toContain("PUBLIC_SKEDULAR_SIGNUP_URL is required");
  });
});
