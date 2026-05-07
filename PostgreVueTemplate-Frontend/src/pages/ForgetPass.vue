<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { ForgetPassword } from "../services/authService";

const router = useRouter();
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

const email = ref("");

async function handleSubmit()
{
	errorMessage.value = null;
	successMessage.value = null;

	if (email.value === "")
	{
		errorMessage.value = "Please enter your email address";
		return;
	}

	isLoading.value = true;
	const [status, result] = await ForgetPassword(email.value);
	isLoading.value = false;

	if (status === 200)
	{
		successMessage.value = "If an account exists with this email, a password reset link has been sent.";
	}
	else
	{
		if (result.Result === "UserNotExist")
			errorMessage.value = "No account found with this email address.";
		else
			errorMessage.value = result.Result || "Request failed";
	}
}
</script>

<template>
	<div>
		<Card flex flex-col bg-surface>
			<p mx-a text-xl>
				{{ $t('forgotPass') }}
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

				<Button color="primary" :loading="isLoading" mt-8 block type="submit">
					{{ $t('sendResetLink') }}
				</Button>

				<Alert v-if="errorMessage" class="mt-3" type="danger" border="start" icon="i-clarity:warning-standard-solid" :text="errorMessage" density="compact" />
				<Alert v-if="successMessage" class="mt-3" type="success" border="start" icon="i-mdi:check-circle" :text="successMessage" density="compact" />
			</form>

			<div flex flex-col items-center pt-4>
				<Button variant="flat" @click="router.push('/signin')">
					{{ $t('backToSignin') }}
				</Button>
				<div flex justify-center>
					<LightSwitch variant="flat" :label="true" />
					<LanguageSwitch variant="flat" :label="true" />
				</div>
			</div>
		</Card>
	</div>
</template>
