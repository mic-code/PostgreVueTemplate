<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { Register } from "../services/authService";

const router = useRouter();
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

const email = ref("");
const password = ref("");
const confirmPassword = ref("");
const showPassword = ref(false);

async function handleSubmit()
{
	errorMessage.value = null;
	successMessage.value = null;

	if (email.value === "" || password.value === "")
	{
		errorMessage.value = "Please fill in all fields";
		return;
	}

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
	const [status, result] = await Register(email.value, password.value);
	isLoading.value = false;

	if (status === 200)
	{
		if (result.Result === "Success")
			successMessage.value = "Registration successful! Please check your email to confirm your account.";
		else if (result.Result === "UserExistNotConfirmed")
			successMessage.value = "Account exists but is not confirmed. A new confirmation email has been sent.";
	}
	else
	{
		if (result.Result === "UserExist")
			errorMessage.value = "An account with this email already exists.";
		else
			errorMessage.value = result.Result || "Registration failed";
	}
}
</script>

<template>
	<div>
		<Card flex flex-col bg-surface>
			<p mx-a text-xl>
				{{ $t('register') }}
			</p>
			<form flex flex-col px-4 @submit.prevent="handleSubmit">
				<div>
					<p>{{ $t('email') }}</p>
					<TextField
						v-model="email"
						density="compact"
						:placeholder="$t('enterEmailAddress')"
						icon="i-mdi-email-outline"
						variant="outlined"
						ariaLabel="email"
						required
						pt-1
					/>
				</div>

				<div mt-2>
					<p>{{ $t('password') }}</p>
					<TextField
						v-model="password"
						:type="showPassword ? 'text' : 'password'"
						:append-icon="showPassword ? 'i-mdi:eye' : 'i-mdi:eye-closed'"
						density="compact"
						:placeholder="$t('enterPassword')"
						icon="i-mdi-lock-outline"
						variant="outlined"
						ariaLabel="password"
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
						ariaLabel="confirm-password"
						required
					/>
				</div>

				<Button color="primary" :loading="isLoading" mt-8 block type="submit">
					{{ $t('register') }}
				</Button>

				<Alert v-if="errorMessage" class="mt-3" type="danger" border="start" icon="i-clarity:warning-standard-solid" :text="errorMessage" density="compact" />
				<Alert v-if="successMessage" class="mt-3" type="success" border="start" icon="i-mdi:check-circle" :text="successMessage" density="compact" />
			</form>

			<div flex flex-col items-center pt-4>
				<Button variant="flat" @click="router.push('/signin')">
					{{ $t('signin') }}
				</Button>
				<div flex justify-center>
					<LightSwitch variant="flat" :label="true" />
					<LanguageSwitch variant="flat" :label="true" />
				</div>
			</div>
		</Card>
	</div>
</template>
