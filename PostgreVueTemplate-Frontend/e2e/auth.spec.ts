import { test, expect } from "@playwright/test";

const TEST_EMAIL = `e2e-test-${Date.now()}@test.com`;
const TEST_PASSWORD = "TestPass123";

// Helper: call the dev-only API to get the token captured by EmailService
async function getDevEmailToken(request: any, email: string): Promise<{ token: string; type: string } | null>
{
	const response = await request.get("/api/Test/GetDevEmailToken?email=" + encodeURIComponent(email));
	if (response.status() === 404) return null;
	const data = await response.json();
	return { token: data.Token, type: data.Type };
}

// Helper: clear dev email store
async function clearDevEmailStore(request: any)
{
	await request.post("/api/Test/ClearDevEmailStore");
}

test.describe("Auth Flow", () =>
{
	test.beforeAll(async ({ request }) =>
	{
		await clearDevEmailStore(request);
	});

	test("Register a new account", async ({ page }) =>
	{
		await page.goto("/register");

		// Fill in the registration form
		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill(TEST_PASSWORD);
		await page.locator('input[aria-label="confirm-password"]').fill(TEST_PASSWORD);

		// Submit
		await page.locator('button[type="submit"]').click();

		// Should show success message
		await expect(page.locator(".alert")).toContainText("Registration successful", { timeout: 10000 });
	});

	test("Cannot sign in without email confirmation", async ({ page }) =>
	{
		await page.goto("/signin");

		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill(TEST_PASSWORD);

		await page.locator('button[type="submit"]').click();

		// Should show "NeedEmailConfirm" error - the i18n key says "Account not activated"
		await expect(page.locator(".alert")).toContainText("activated", { timeout: 10000 });
	});

	test("Confirm email via dev token", async ({ page, request }) =>
	{
		// Get the token from the dev API
		const record = await getDevEmailToken(request, TEST_EMAIL);
		expect(record).not.toBeNull();
		expect(record!.type).toBe("confirm");

		// Navigate to the confirm email page with the token
		await page.goto(`/confirmEmail?email=${encodeURIComponent(TEST_EMAIL)}&token=${encodeURIComponent(record!.token)}`);

		// Should show success message
		await expect(page.locator(".alert")).toContainText("confirmed successfully", { timeout: 10000 });
	});

	test("Sign in after email confirmation", async ({ page }) =>
	{
		await page.goto("/signin");

		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill(TEST_PASSWORD);

		await page.locator('button[type="submit"]').click();

		// Should redirect away from signin (to home page)
		await expect(page).toHaveURL(/^(?!.*signin).*$/, { timeout: 10000 });
	});

	test("Sign out", async ({ page }) =>
	{
		// First sign in
		await page.goto("/signin");
		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill(TEST_PASSWORD);
		await page.locator('button[type="submit"]').click();
		await expect(page).toHaveURL(/^(?!.*signin).*$/, { timeout: 10000 });

		// Sign out via API
		const response = await page.request.get("/api/Auth/Signout");
		expect(response.ok()).toBeTruthy();

		// Navigate to signin page
		await page.goto("/signin");
		await expect(page).toHaveURL(/signin/);
	});

	test("Forgot password flow", async ({ page, request }) =>
	{
		await clearDevEmailStore(request);

		await page.goto("/forgetPass");

		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('button[type="submit"]').click();

		// Should show success message
		await expect(page.locator(".alert")).toContainText("password reset link has been sent", { timeout: 10000 });

		// Get the reset token from the dev API
		const record = await getDevEmailToken(request, TEST_EMAIL);
		expect(record).not.toBeNull();
		expect(record!.type).toBe("reset");

		// Navigate to the reset password page with the token
		await page.goto(`/resetPass?email=${encodeURIComponent(TEST_EMAIL)}&token=${encodeURIComponent(record!.token)}`);

		const NEW_PASSWORD = "NewPass456";
		await page.locator('input[aria-label="new-password"]').fill(NEW_PASSWORD);
		await page.locator('input[aria-label="confirm-new-password"]').fill(NEW_PASSWORD);

		await page.locator('button[type="submit"]').click();

		// Should show success message
		await expect(page.locator(".alert")).toContainText("reset successfully", { timeout: 10000 });
	});

	test("Sign in with new password after reset", async ({ page }) =>
	{
		await page.goto("/signin");

		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill("NewPass456");

		await page.locator('button[type="submit"]').click();

		// Should redirect away from signin
		await expect(page).toHaveURL(/^(?!.*signin).*$/, { timeout: 10000 });
	});

	test("Register with existing email shows error", async ({ page }) =>
	{
		await page.goto("/register");

		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill(TEST_PASSWORD);
		await page.locator('input[aria-label="confirm-password"]').fill(TEST_PASSWORD);

		await page.locator('button[type="submit"]').click();

		// Should show error about existing account
		await expect(page.locator(".alert")).toContainText("already exists", { timeout: 10000 });
	});

	test("Sign in with wrong password shows error", async ({ page }) =>
	{
		await page.goto("/signin");

		await page.locator('input[aria-label="email"]').fill(TEST_EMAIL);
		await page.locator('input[aria-label="password"]').fill("WrongPassword999");

		await page.locator('button[type="submit"]').click();

		// Should show incorrect credential error
		await expect(page.locator(".alert")).toContainText("Incorrect", { timeout: 10000 });
	});

	test("Forgot password for non-existent email shows error", async ({ page }) =>
	{
		await page.goto("/forgetPass");

		await page.locator('input[aria-label="email"]').fill("nonexistent@test.com");
		await page.locator('button[type="submit"]').click();

		// Backend returns UserNotExist error for non-existent email
		await expect(page.locator(".alert")).toContainText("No account found", { timeout: 10000 });
	});

	test("Navigate between auth pages", async ({ page }) =>
	{
		// Signin -> Register
		await page.goto("/signin");
		await page.locator('text=Register').first().click();
		await expect(page).toHaveURL(/register/);

		// Register -> Signin
		await page.locator('text=Sign In').first().click();
		await expect(page).toHaveURL(/signin/);

		// Signin -> Forgot Password
		await page.locator('text=Forgot Password').first().click();
		await expect(page).toHaveURL(/forgetPass/);

		// Forgot Password -> Back to Sign In
		await page.locator('text=Back to Sign In').first().click();
		await expect(page).toHaveURL(/signin/);
	});
});
