import { App, DirectiveBinding } from "vue";
import { useTooltipStore, TooltipContent, TooltipPlacement } from "../stores/tooltipStore";

// Extend HTMLElement to add our custom properties
interface TooltipElement extends HTMLElement {
	_tooltipMouseEnter?: () => void;
	_tooltipMouseLeave?: () => void;
	_tooltipClick?: () => void;
}

export const vTooltip = {
	mounted(el: TooltipElement, binding: DirectiveBinding)
	{
		const store = useTooltipStore();

		el._tooltipMouseEnter = () =>
		{
			if (!binding.value) return;

			const data: TooltipContent = typeof binding.value === "string"
				? { text: binding.value }
				: { ...binding.value }; // Clone to avoid mutating original

			// Handle Modifiers (v-tooltip.bottom)
			let placement: TooltipPlacement = "bottom"; // Default
			if (binding.modifiers.top) placement = "top";
			else if (binding.modifiers.left) placement = "left";
			else if (binding.modifiers.right) placement = "right";
			else if (binding.modifiers.bottom) placement = "bottom";

			// If data object doesn't specify placement, use modifier or default
			if (!data.placement)
			{
				data.placement = placement;
			}

			store.show(data, el.getBoundingClientRect());
		};

		el._tooltipMouseLeave = () =>
		{
			store.hide();
		};
		
		el._tooltipClick = () => 
		{
			store.hide();
		};

		el.addEventListener("mouseenter", el._tooltipMouseEnter);
		el.addEventListener("mouseleave", el._tooltipMouseLeave);
		el.addEventListener("click", el._tooltipClick);
	},
	updated(el: TooltipElement, binding: DirectiveBinding)
	{
		// We could update the store if the element is currently being hovered
		// But for now, simple implementation
	},
	beforeUnmount(el: TooltipElement)
	{
		if (el._tooltipMouseEnter) el.removeEventListener("mouseenter", el._tooltipMouseEnter);
		if (el._tooltipMouseLeave) el.removeEventListener("mouseleave", el._tooltipMouseLeave);
		if (el._tooltipClick) el.removeEventListener("click", el._tooltipClick);
	}
};

export default (app: App) =>
{
	app.directive("tooltip", vTooltip);
};
