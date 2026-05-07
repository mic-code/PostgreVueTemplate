<script setup lang="ts">
import { useTooltipStore } from "../../stores/tooltipStore";
import { ref, watch, nextTick } from "vue";

const store = useTooltipStore();
const tooltipRef = ref<HTMLElement | null>(null);
const coords = ref({ top: 0, left: 0 });

// Recalculate position whenever the tooltip is shown or content changes
watch(() => [store.isVisible, store.targetRect, store.content], async ([visible]) =>
{
	if (visible && store.targetRect)
	{
		await nextTick(); // Wait for content to render to get correct size

		if (tooltipRef.value)
		{
			const tooltip = tooltipRef.value.getBoundingClientRect();
			const target = store.targetRect;
			const gap = 10;
			const placement = store.content.placement || "bottom";

			let top = 0;
			let left = 0;

			// --- Logic based on placement ---
			if (placement === "top")
			{
				top = target.top - tooltip.height - gap;
				left = target.left + (target.width / 2) - (tooltip.width / 2);

				// Flip to bottom if clipping top
				if (top < 0) top = target.bottom + gap;
			}
			else if (placement === "bottom")
			{
				top = target.bottom + gap;
				left = target.left + (target.width / 2) - (tooltip.width / 2);

				// Flip to top if clipping bottom
				if (top + tooltip.height > window.innerHeight) top = target.top - tooltip.height - gap;
			}
			else if (placement === "left")
			{
				top = target.top + (target.height / 2) - (tooltip.height / 2);
				left = target.left - tooltip.width - gap;

				// Flip to right if clipping left
				if (left < 0) left = target.right + gap;
			}
			else if (placement === "right")
			{
				top = target.top + (target.height / 2) - (tooltip.height / 2);
				left = target.right + gap;

				// Flip to left if clipping right
				if (left + tooltip.width > window.innerWidth) left = target.left - tooltip.width - gap;
			}

			// --- Common Clamping ---
			// Clamp horizontal (keep on screen)
			left = Math.max(10, Math.min(window.innerWidth - tooltip.width - 10, left));

			// Clamp vertical (keep on screen)
			top = Math.max(10, Math.min(window.innerHeight - tooltip.height - 10, top));

			coords.value = { top, left };
		}
	}
});
</script>

<template>
	<Teleport to="body">
		<Transition name="tooltip-fade">
			<div
				v-if="store.isVisible"
				ref="tooltipRef"
				class="global-tooltip"
				:style="{ top: `${coords.top}px`, left: `${coords.left}px` }"
			>
				<!-- Case 1: Complex Object -->
				<div v-if="store.content.title || store.content.image" class="rich-content">
					<div v-if="store.content.title" class="tooltip-title">{{ store.content.title }}</div>
					<img v-if="store.content.image" :src="store.content.image" class="tooltip-img" >
					<div v-if="store.content.html" v-html="store.content.html" class="tooltip-body"></div>
					<div v-else-if="store.content.text" class="tooltip-body">{{ store.content.text }}</div>
				</div>

				<!-- Case 2: Simple Text -->
				<span v-else>
					{{ store.content.text || store.content }}
				</span>
			</div>
		</Transition>
	</Teleport>
</template>

<style scoped>
.global-tooltip {
	position: fixed;
	z-index: 2000;
	pointer-events: none; /* Non-interactable */
	background: var(--surface2, #333);
	color: var(--onsurface, #fff);
	border: 1px solid rgb(128 128 128 / 20%);
	box-shadow: 0 4px 12px rgb(0 0 0 / 20%);
	border-radius: 6px;
	padding: 8px 12px;
	font-size: 0.85rem;
	max-width: 300px;
	white-space: pre-line; /* Handle newlines in text */
}

/* Rich Content Styles */
.tooltip-title {
	font-weight: 700;
	margin-bottom: 4px;
	border-bottom: 1px solid rgb(128 128 128 / 20%);
	padding-bottom: 4px;
}

.tooltip-img {
	width: 100%;
	height: auto;
	border-radius: 4px;
	margin-bottom: 6px;
	display: block;
}

.tooltip-body {
	opacity: 0.9;
	line-height: 1.4;
}

.tooltip-fade-enter-active,
.tooltip-fade-leave-active {
	transition: opacity 0.15s ease;
}

.tooltip-fade-enter-from,
.tooltip-fade-leave-to {
	opacity: 0;
}
</style>
