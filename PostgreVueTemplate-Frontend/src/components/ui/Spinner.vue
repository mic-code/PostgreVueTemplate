<script setup lang="ts">
import { computed } from "vue";

const props = withDefaults(defineProps<{
	progress?: number | null;
	size?: number;
	strokeWidth?: number;
}>(),
{
	progress: null,
	size: 48,
	strokeWidth: 4,
});
//remove the size props and fill parent element

const isDeterminate = computed(() => typeof props.progress === "number" && props.progress >= 0 && props.progress <= 100);

const radius = computed(() => (props.size - props.strokeWidth) / 2);
const circumference = computed(() => 2 * Math.PI * radius.value);
const strokeDashoffset = computed(() =>
{
	if (isDeterminate.value)
	{
		const progressValue = Math.max(0, Math.min(1, props.progress!));
		return circumference.value * (1 - progressValue);
	}
	return null;
});

const strokeDasharray = computed(() =>
{
	if (isDeterminate.value)
		return circumference.value;
	return `${circumference.value * 0.25} ${circumference.value * 0.75}`;
});

const viewBox = computed(() => `0 0 ${props.size} ${props.size}`);
const center = computed(() => props.size / 2);
</script>

<template>
	<div :style="{ width: `${size}px`, height: `${size}px` }" relative	>
		<svg :viewBox="viewBox" :class="['spinner-svg', { 'spinner-indeterminate-animation': !isDeterminate }]">
			<circle
				v-if="isDeterminate"
				class="spinner-track"
				:cx="center"
				:cy="center"
				:r="radius"
				:stroke-width="strokeWidth"
			/>
			<circle
				:class="['spinner-progress', { 'spinner-indeterminate': !isDeterminate }]"
				:cx="center"
				:cy="center"
				:r="radius"
				:stroke-width="strokeWidth"
				:stroke-dasharray="strokeDasharray"
				:stroke-dashoffset="strokeDashoffset"
			/>
		</svg>
		<div absolute top-0 h-full w-full flex flex-col items-center justify-center >
			<p class="spinner-text">
				<span v-if="isDeterminate" >
					{{ Math.round(progress!*100) }}%
				</span>
				<span>
					<slot></slot>
				</span>
			</p>
		</div>
	</div>
</template>

<style scoped>
.spinner-svg {
	transform: rotate(-90deg);
}

.spinner-track {
	fill: none;
	stroke: var(--surface);
}

.spinner-progress {
	fill: none;
	stroke: var(--primary);
	stroke-linecap: round;
	transition: stroke-dashoffset 0.3s;
}

.spinner-indeterminate-animation {
	animation: spinner-rotate 2s linear infinite;
	transform: rotate(0deg);
}

.spinner-indeterminate {
	stroke-dashoffset: 0;
	transform-origin: center;
}

.spinner-text {
	font-size: 1em;
	color: var(--onsurface);
}

@keyframes spinner-rotate {
	100% {
		transform: rotate(360deg);
	}
}
</style>
