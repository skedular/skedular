import { spawn } from "node:child_process";
import { describe, expect, it } from "vitest";
import { publicUrlEnvironment, publicUrlFixtures } from "./public-url-fixtures";

function runBuild(environment: NodeJS.ProcessEnv) {
  return new Promise<{ code: number | null; stdout: string; stderr: string }>(
    (resolve) => {
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
    },
  );
}

describe("public website build diagnostics", () => {
  it("requires all public destination URLs and hides full URL values", async () => {
    const result = await runBuild({ ...process.env, ...publicUrlEnvironment });

    expect(result.code, `${result.stdout}\n${result.stderr}`).toBe(0);
    expect(result.stdout).toContain('"event":"public-web.build.complete"');
    const pageCount = Number(
      result.stdout.match(/"pageCount":(\d+)/)?.[1] ?? 0,
    );
    expect(pageCount).toBeGreaterThanOrEqual(18);
    expect(result.stdout).toMatch(/"outputBytes":\d+/);

    const output = `${result.stdout}${result.stderr}`;
    expect(output).not.toContain(publicUrlFixtures.appUrl);
    expect(output).not.toContain(publicUrlFixtures.signupUrl);
    expect(output).not.toContain(publicUrlFixtures.demoUrl);
    expect(output).not.toContain(publicUrlFixtures.becomeHostUrl);
    expect(output).not.toContain(publicUrlFixtures.slackInstallUrl);
  });

  it.each([
    "PUBLIC_SKEDULAR_APP_URL",
    "PUBLIC_SKEDULAR_SIGNUP_URL",
    "PUBLIC_SKEDULAR_HOST_APP_URL",
    "PUBLIC_SKEDULAR_DEMO_URL",
    "PUBLIC_SKEDULAR_BECOME_HOST_URL",
    "PUBLIC_SKEDULAR_SLACK_INSTALL_URL",
  ] as const)("fails clearly when %s is missing", async (name) => {
    const environment = { ...process.env, ...publicUrlEnvironment, [name]: "" };

    const result = await runBuild(environment);

    expect(result.code).not.toBe(0);
    expect(`${result.stdout}${result.stderr}`).toContain(`${name} is required`);
  });
});
