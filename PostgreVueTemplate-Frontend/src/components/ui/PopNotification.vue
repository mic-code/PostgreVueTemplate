<script setup lang="ts">
import { ref, onUnmounted } from "vue";

interface Notification {
  id: number
  message: string
  type: "success" | "error" | "warning" | "info"
  timeout?: number
  leaving: boolean
  timer?: ReturnType<typeof setTimeout>
  paused: boolean
  remaining?: number
}

const notifications = ref<Notification[]>([]);
let nextId = 0;

const DEFAULT_TIMEOUT = 5000;

const addNotification = (message: string, type: Notification["type"] = "info", timeout: number = DEFAULT_TIMEOUT) =>
{
	const id = nextId++;
	const notification: Notification = {
		id,
		message,
		type,
		timeout,
		leaving: false,
		paused: false
	};

	notifications.value.push(notification);
	startTimeout(notification);
};

const removeNotification = (id: number) =>
{
	const index = notifications.value.findIndex(n => n.id === id);
	if (index !== -1)
	{
		const notification = notifications.value[index];
		if (notification.timer)
		{
			clearTimeout(notification.timer);
		}
		notification.leaving = true;
		setTimeout(() =>
		{
			const removeIndex = notifications.value.findIndex(n => n.id === id);
			if (removeIndex !== -1)
			{
				notifications.value.splice(removeIndex, 1);
			}
		}, 300);
	}
};

const startTimeout = (notification: Notification) =>
{
	notification.timer = setTimeout(() =>
	{
		removeNotification(notification.id);
	}, notification.timeout);
};

const pauseTimeout = (id: number) =>
{
	const notification = notifications.value.find(n => n.id === id);
	if (notification && notification.timer && !notification.paused)
	{
		clearTimeout(notification.timer);
		notification.paused = true;
		if (notification.timeout && notification.remaining === undefined)
		{
			notification.remaining = notification.timeout - (Date.now() - notification.timer._idleStart);
		}
	}
};

const resumeTimeout = (id: number) =>
{
	const notification = notifications.value.find(n => n.id === id);
	if (notification && notification.paused && notification.remaining !== undefined)
	{
		notification.timer = setTimeout(() =>
		{
			removeNotification(notification.id);
		}, notification.remaining);
		notification.paused = false;
		notification.remaining = undefined;
	}
};

// Export the addNotification function to be used globally
defineExpose({
	addNotification
});

onUnmounted(() =>
{
	notifications.value.forEach(notification =>
	{
		if (notification.timer)
		{
			clearTimeout(notification.timer);
		}
	});
});
</script>

<template>
	<div class="fixed right-4 top-4 z-1000 flex flex-col gap-2">
		<div
			v-for="notification in notifications"
			:key="notification.id"
			class="max-w-[90vw] rounded-lg p-4 shadow-lg transition-all duration-300 sm:max-w-md"
			:class="{
				'bg-success text-onsuccess': notification.type === 'success',
				'bg-warning text-onwarning': notification.type === 'warning',
				'bg-danger text-ondanger': notification.type === 'error',
				'bg-secondary text-onsecondary': notification.type === 'info',
				'opacity-0 translate-x-full': notification.leaving,
				'opacity-100 translate-x-0': !notification.leaving
			}"
			@mouseenter="pauseTimeout(notification.id)"
			@mouseleave="resumeTimeout(notification.id)"
		>
			<div class="flex items-center justify-between">
				<span>{{ notification.message }}</span>
				<Button
					variant="flat"
					icon="i-mdi:close"
					@click="removeNotification(notification.id)"
					class="ml-4 text-inherit opacity-70 hover:opacity-100"
				/>
			</div>
		</div>
	</div>
</template>