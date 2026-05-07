<script setup lang="ts">
import { useAppStore } from "../../stores/appStore";

const appStore = useAppStore();

function handleModeChange()
{
	const isDark = appStore.colorMode == "dark";
	appStore.colorMode = isDark ? "light" : "dark";
}

</script>

<template>
	<Button :icon="appStore.colorMode=='dark'?'i-ri-moon-fill':'i-ri-sun-fill'" @click="handleModeChange"/>
</template>