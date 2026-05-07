
<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { defaultSize } from "../../utilities/sizeUtility";

defineProps({
	variant: String,
	size: { type: Number, default: defaultSize },
});
const i18n = useI18n();

const availableLocales = i18n.availableLocales;

function setLocale(locale: string)
{
	i18n.locale.value = locale;
	localStorage.setItem("lang", locale);
}

</script>

<template>
	<Button :size="size" :variant="variant" id="langSwitch" icon="i-clarity:language-line"></Button>
	<Menu element-id="langSwitch" >
		<div flex flex-col>
			<Button v-for="lang in availableLocales" :key="lang" @click="setLocale(lang)" round="0" variant="flat">{{ $t('languageName', 1, { locale: lang }) }}</Button>
		</div>
	</Menu>
</template>
