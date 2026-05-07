<script setup lang="ts">
withDefaults(defineProps<{
  progress?: number | null; // 0-1
}>(), {
	progress: null,
});
</script>

<template>
	<div v-if="progress != null" flex items-center	>
		<div h-2 w-full flex rounded-full bg-surface >
			<div
				class="h-2 rounded-full bg-primary transition-all duration-300"
				:style="{ width: `${progress * 100}%` }"
				role="progressbar"
				:aria-valuenow="progress"
				aria-valuemin="0"
				aria-valuemax="1"
			/>
		</div>
		<div ma-0 w-14 text-center font-size-sm text-onsurface >
			{{ (progress * 100).toFixed(0) }}%
		</div>
	</div>
	<div v-else w-full flex items-center	>
		<div relative h-2 w-full overflow-hidden rounded-full bg-surface>
			<div class="indeterminate-bar absolute top-0 h-full w-1/2 bg-primary" />
		</div>
	</div>
</template>

<style scoped>
.transition-all {
	transition-property: all;
	transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
	transition-duration: 150ms;
}

.indeterminate-bar {
	animation: indeterminate-animation 2s infinite linear;
}

@keyframes indeterminate-animation {
	from {
		left: -50%;
	}

	to {
		left: 100%;
	}
}
</style>
