// scripts/build-cjs.mjs
import esbuild from "esbuild";

await esbuild.build({
  entryPoints: ["src/index.ts"], // CLI 入口
  bundle: true,
  platform: "node",
  target: "node18",
  format: "cjs",
  outfile: "dist/mgrep.cjs",
  banner: {
    js: "#!/usr/bin/env node",
  },
  external: [
    "@modelcontextprotocol/sdk",
    "@modelcontextprotocol/sdk/*",
    "better-auth",
    "better-auth/*",
    "better-auth/client",
    "better-auth/client/*",
    "better-auth/client/plugins",
  ],
});
