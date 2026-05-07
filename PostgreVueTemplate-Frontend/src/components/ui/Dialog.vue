<script setup lang="ts">
import { watch, onUnmounted } from "vue";

const model = defineModel<boolean>({ default: false });

const props = defineProps({
	modal: Boolean
});

let originalBodyOverflow = "";
let originalBodyPaddingRight = "";

watch(model, (newVal) =>
{
	if (typeof window === "undefined") return;

	if (newVal)
	{
		// Save current inline styles
		const bodyStyle = document.body.style;
		originalBodyOverflow = bodyStyle.overflow;
		originalBodyPaddingRight = bodyStyle.paddingRight;

		const scrollbarWidth = window.innerWidth - document.documentElement.clientWidth;
		bodyStyle.overflow = "hidden";
		bodyStyle.paddingRight = `${scrollbarWidth}px`;
	}
	else
	{
		// Restore original inline styles
		document.body.style.overflow = originalBodyOverflow;
		document.body.style.paddingRight = originalBodyPaddingRight;
	}
});

onUnmounted(() =>
{
	if (model.value)
	{
		document.body.style.overflow = originalBodyOverflow;
		document.body.style.paddingRight = originalBodyPaddingRight;
	}
});

function handleOverlayClick(event: MouseEvent)
{
	// Only close if it's not a modal dialog
	if (!props.modal && event.target === event.currentTarget)
		model.value = false;
}
</script>

<template>
	<Teleport to="body">
		<Transition name="dialog-fade">
			<div v-if="model" class="dialog-overlay" @click="handleOverlayClick">
				<slot></slot>
			</div>
		</Transition>
	</Teleport>
</template>

<style scoped>
.dialog-overlay {
	position: fixed;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	background-color: rgb(0 0 0 / 50%);
	display: flex;
	justify-content: center;
	align-items: center;
	z-index: 9999;
}

.dialog-fade-enter-active {
	transition: opacity 0.2s ease-in-out;
}

.dialog-fade-leave-active {
	transition: opacity 0.2s ease-in-out;
	pointer-events: none;
}

.dialog-fade-enter-from,
.dialog-fade-leave-to {
	opacity: 0;
}

.dialog-fade-enter-active .dialog-content,
.dialog-fade-leave-active .dialog-content {
	transition: transform 0.2s ease-in-out;
}

.dialog-fade-enter-from .dialog-content,
.dialog-fade-leave-to .dialog-content {
	transform: scale(0.95);
}
</style>
