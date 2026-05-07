import { defineStore } from "pinia";
import { ref } from "vue";

export type TooltipPlacement = "top" | "bottom" | "left" | "right";

export interface TooltipContent {
	text?: string;       // Simple text
	title?: string;      // Optional Header
	html?: string;       // For bolding, colors, etc.
	image?: string;      // Optional image URL
	placement?: TooltipPlacement; // Preferred position
}

export const useTooltipStore = defineStore("tooltip", () =>
{
	const isVisible = ref(false);
	const content = ref<TooltipContent>({});
	const targetRect = ref<DOMRect | null>(null);

	function show(data: TooltipContent, rect: DOMRect)
	{
		content.value = data;
		targetRect.value = rect;
		isVisible.value = true;
	}

	function hide()
	{
		isVisible.value = false;
	}

	return { isVisible, content, targetRect, show, hide };
});
