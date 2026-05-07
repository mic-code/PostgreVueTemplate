<template>
	<router-view />
	<PopNotification ref="notificationRef" />
	<GlobalTooltip />
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import PopNotification from "./components/ui/PopNotification.vue";

const notificationRef = ref();

onMounted(() =>
{
	// Expose the notification methods globally
	if (notificationRef.value)
	{
		// Store a reference to the notification component for global access
		(window as any).$notification = notificationRef.value;
	}
});
</script>
