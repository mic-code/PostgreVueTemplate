<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import { ConfirmEmail } from "../services/authService";

const router = useRouter();
const route = useRoute();
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);
const successMessage = ref<string | null>(null);

const email = ref("");
const token = ref("");

onMounted(async () =>
{
	email.value = (route.query.email as string) || "";
	token.value = (route.query.token as string) || "";

	if (email.value && token.value)
	{
		isLoading.value = true;
		const [status, result] = await ConfirmEmail(email.value, token.value);
		isLoading.value = false;

		if (status === 200)
		{
			successMessage.value = "Email confirmed successfully! You can now sign in.";
		}
		else
		{
			errorMessage.value = result.Result || "Email confirmation failed.";
		}
	}
	else
	{
		errorMessage.value = "Invalid confirmation link.";
	}
});
</script>

<template>
	<div>
		<Card flex flex-col bg-surface>
			<p mx-a text-xl>
				{{ $t('confirmEmail') }}
			</p>
			<div flex flex-col px-4>
				<Alert v-if="isLoading" class="mt-3" type="info" border="start" icon="i-mdi-loading" text="Confirming your email..." density="compact" />
				<Alert v-if="errorMessage" class="mt-3" type="danger" border="start" icon="i-clarity:warning-standard-solid" :text="errorMessage" density="compact" />
				<Alert v-if="successMessage" class="mt-3" type="success" border="start" icon="i-mdi:check-circle" :text="successMessage" density="compact" />
			</div>

			<div flex flex-col items-center pt-4>
				<Button v-if="successMessage" variant="flat" @click="router.push('/signin')">
					{{ $t('signin') }}
				</Button>
				<Button v-else-if="errorMessage" variant="flat" @click="router.push('/signin')">
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