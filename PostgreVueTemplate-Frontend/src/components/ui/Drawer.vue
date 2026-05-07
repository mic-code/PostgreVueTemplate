<script setup lang="ts">
import { computed } from "vue";

const props = defineProps({
	scrollDirection: {
		type: String,
		default: "up",
		validator: (value: string) => ["up", "down", "left", "right"].includes(value)
	},
	darkenBackground: {
		type: Boolean,
		default: true
	},
	maxWidthHeight: {
		type: Number,
		default: 90
	}
});

const isOpen = defineModel<boolean>();

function closeDrawer()
{
	isOpen.value = false;
}

const transitionName = computed(() =>
{
	switch (props.scrollDirection)
	{
		case "down":
			return "slide-down";
		case "left":
			return "slide-left";
		case "right":
			return "slide-right";
		case "up":
		default:
			return "slide-up";
	}
});

const panelStyle = computed(() =>
{
	switch (props.scrollDirection)
	{
		case "down":
			return {
				top: 0,
				left: 0,
				width: "100%",
				maxHeight: `${props.maxWidthHeight}vh`,
				boxShadow: "0 2px 16px rgb(0 0 0 / 10%)",
			};
		case "left":
			return {
				top: 0,
				right: 0,
				height: "100%",
				maxWidth: `${props.maxWidthHeight}vw`,
				boxShadow: "-2px 0 16px rgb(0 0 0 / 10%)",
			};
		case "right":
			return {
				top: 0,
				left: 0,
				height: "100%",
				maxWidth: `${props.maxWidthHeight}vw`,
				boxShadow: "2px 0 16px rgb(0 0 0 / 10%)",
			};
		case "up":
		default:
			return {
				bottom: 0,
				left: 0,
				width: "100%",
				maxHeight: `${props.maxWidthHeight}vh`,
				boxShadow: "0 -2px 16px rgb(0 0 0 / 10%)",
			};
	}
});
</script>

<template>
	<Transition name="fade">
		<div v-if="isOpen && darkenBackground" class="drawer-overlay" @click="closeDrawer" @touchstart="closeDrawer"></div>
	</Transition>
	<Transition :name="transitionName">
		<div v-if="isOpen" class="drawer-panel" :style="panelStyle">
			<slot></slot>
		</div>
	</Transition>
</template>



<style scoped>
.drawer-overlay {
	position: fixed;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	background-color: rgb(0 0 0 / 50%);
	z-index: 1000;
}

.drawer-panel {
	position: fixed;
	background-color: var(--surface3);
	overflow-y: auto;
	box-sizing: border-box;
	z-index: 1001;
}

/* Transitions */
.fade-enter-active,
.fade-leave-active {
	transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
	opacity: 0;
}

.slide-up-enter-active,
.slide-up-leave-active,
.slide-down-enter-active,
.slide-down-leave-active,
.slide-left-enter-active,
.slide-left-leave-active,
.slide-right-enter-active,
.slide-right-leave-active {
	transition: transform 0.3s ease-out;
}

.slide-up-enter-from,
.slide-up-leave-to {
	transform: translateY(100%);
}

.slide-down-enter-from,
.slide-down-leave-to {
	transform: translateY(-100%);
}

.slide-left-enter-from,
.slide-left-leave-to {
	transform: translateX(100%);
}

.slide-right-enter-from,
.slide-right-leave-to {
	transform: translateX(-100%);
}
</style>
