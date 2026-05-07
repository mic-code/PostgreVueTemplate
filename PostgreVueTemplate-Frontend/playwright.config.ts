import { defineConfig } from "@playwright/test";

export default defineConfig({
	testDir: "./e2e",
	timeout: 30000,
	expect: {
		timeout: 10000,
	},
	retries: 0,
	use: {
		baseURL: "https://127.0.0.1:3050",
		trace: "on-first-retry",
		headless: true,
		ignoreHTTPSErrors: true,
	},
	projects: [
		{
			name: "chromium",
			use: { browserName: "chromium" },
		},
	],
});