<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { ResetPassword } from "../services/authService";

const router = useRouter();
const route = useRoute();
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const token = ref("");
const showPassword = ref(false);

onMounted(() =>
{
	email.value = (route.query.email as string) || "";
	token.value = (route.query.token as string) || "";

	if (!email.value || !token.value)
	{
		errorMessage.value = "Invalid reset link. Please request a new password reset.";
	}
});

async function handleSubmit()
{
	errorMessage.value = null;
	successMessage.value = null;

	if (password.value.length < 6)
	{
		errorMessage.value = "Password must be at least 6 characters";
		return;
	}

	if (password.value !== confirmPassword.value)
	{
		errorMessage.value = "Passwords do not match";
		return;
	}

	isLoading.value = true;
	const [status, result] = await ResetPassword(email.value, password.value, token.value);
	isLoading.value = false;

	if (status === 200)
	{
		successMessage.value = "Password has been reset successfully. You can now sign in.";
	}
	else
	{
		if (result.Result === "InvalidToken")
			errorMessage.value = "The reset token is invalid or has expired. Please request a new one.";
		else
			errorMessage.value = result.Result || "Reset failed";
	}
}
</script>

<template>
	<div>
		<Card flex flex-col bg-surface>
			<p mx-a text-xl>
				{{ $t('resetPassword') }}
			</p>
			<form flex flex-col px-4 @submit.prevent="handleSubmit">
				<div>
					<p>{{ $t('email') }}</p>
					<TextField
						v-model="email"
						density="compact"
						icon="i-mdi-email-outline"
						variant="outlined"
						ariaLabel="email"
						disabled
						pt-1
					/>
				</div>

				<div mt-2>
					<p>{{ $t('newPassword') }}</p>
					<TextField
						v-model="password"
						:type="showPassword ? 'text' : 'password'"
						:append-icon="showPassword ? 'i-mdi:eye' : 'i-mdi:eye-closed'"
						density="compact"
						:placeholder="$t('enterNewPassword')"
						icon="i-mdi-lock-outline"
						variant="outlined"
						ariaLabel="new-password"
						required
						@click:append="showPassword = !showPassword"
					/>
				</div>

				<div mt-2>
					<p>{{ $t('confirmPassword') }}</p>
					<TextField
						v-model="confirmPassword"
						:type="showPassword ? 'text' : 'password'"
						density="compact"
						:placeholder="$t('confirmPasswordPlaceholder')"
						icon="i-mdi-lock-check-outline"
						variant="outlined"
						ariaLabel="confirm-new-password"
						required
					/>
				</div>

				<Button color="primary" :loading="isLoading" mt-8 block type="submit">
					{{ $t('resetPassword') }}
				</Button>

				<Alert v-if="errorMessage" class="mt-3" type="danger" border="start" icon="i-clarity:warning-standard-solid" :text="errorMessage" density="compact" />
				<Alert v-if="successMessage" class="mt-3" type="success" border="start" icon="i-mdi:check-circle" :text="successMessage" density="compact" />
			</form>

			<div flex flex-col items-center pt-4>
				<Button v-if="successMessage" variant="flat" @click="router.push('/signin')">
					{{ $t('signin') }}
				</Button>
				<Button v-else variant="flat" @click="router.push('/forgetPass')">
					{{ $t('requestNewResetLink') }}
				</Button>
				<div flex justify-center>
					<LightSwitch variant="flat" :label="true" />
					<LanguageSwitch variant="flat" :label="true" />
				</div>
			</div>
		</Card>
	</div>
</template>