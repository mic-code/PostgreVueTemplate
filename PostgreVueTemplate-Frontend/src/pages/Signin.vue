<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "../stores/authStore";

const showPassword = ref(false);
const store = useAuthStore();
const router = useRouter();
const isLoading = ref<boolean>();

const email = ref<string>("");
const password = ref<string>("");

async function handleSubmit()
{
	if(email.value == "" || password.value == "")
		return;

	isLoading.value = true;
	await store.signin(email.value, password.value);

	if (store.isLoggedIn)
	{
		const b = router.options.history.state.back as string;
		if(b != null && !isAuthPage(b))
			router.go(-1);
		else
			router.replace({ path: "/" });
	}
	isLoading.value = false;
}

function isAuthPage(b)
{
	return b.indexOf("/register") === 0
		|| b.indexOf("/forgetPass") === 0
		|| b.indexOf("/resetPass") === 0
		|| b.indexOf("/confirmEmail") === 0;
}

async function handleForgetPass()
{
	router.replace({ path: "/forgetPass" });
}

</script>

<template>
	<div>
		<Card flex flex-col bg-surface >
			<p mx-a text-xl>
				Sign in
			</p>
			<form flex flex-col px-4 @submit.prevent="handleSubmit">
				<div>
					<p>
						{{ $t('email') }}
					</p>
					<TextField
						v-model="email"
						density="compact"
						:placeholder="$t('enterEmailAddress')"
						icon="i-mdi-email-outline"
						variant="outlined"
						aria-label="email"
						required
						pt-1
					/>
				</div>

				<div mt-2 flex flex-col>

					<div flex>
						<p>
							{{ $t('password') }}
						</p>
						<Button ms-a variant="flat" self-center @click="handleForgetPass">
							{{ $t('forgotPass') }}
						</Button>
					</div>
					<TextField
						v-model="password"
						:type="showPassword ? 'text' : 'password'"
						:append-icon="showPassword?'i-mdi:eye':'i-mdi:eye-closed'"
						density="compact"
						:placeholder="$t('enterPassword')"
						icon="i-mdi-lock-outline"
						variant="outlined"
						aria-label="password"
						required
						@click:append="showPassword=!showPassword"
					/>
				</div>


				<Button color="primary" :loading="isLoading" mt-8 block type="submit">
					{{ $t('signin') }}
				</Button>
				<Alert v-if="store.signinError" class="mt-3" type="danger" border="start" icon="i-clarity:warning-standard-solid" :text="$t(store.signinError as any)" density="compact" />
			</form>


			<div flex flex-col items-center pt-4 >
				<Button variant="flat" @click="router.push('/register')">
					{{ $t('register') }}
				</Button>
				<div flex justify-center>
					<LightSwitch variant="flat" :label="true" />
					<LanguageSwitch variant="flat" :label="true" />
				</div>
			</div>
		</Card>
	</div>
</template>

<style>
input:autofill {
	box-shadow: 0 0 0 40px inset rgb(var(--v-theme-surface)) !important;
}
</style>