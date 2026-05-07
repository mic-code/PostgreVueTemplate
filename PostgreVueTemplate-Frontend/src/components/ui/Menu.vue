<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from "vue";

const props = withDefaults(defineProps<{
	elementId: string;
	matchWidth?: boolean;
	attachListener?: boolean;
	disabled?: boolean;
}>(), {
	attachListener: true,
	disabled: false
});

const isOpen = defineModel<boolean>("isOpen", { default: false });

const menuRef = ref<HTMLElement | null>(null);
const menuTop = ref(0);
const menuLeft = ref(0);
const menuWidth = ref<string | undefined>(undefined);

const toggleMenu = () =>
{
	if (props.disabled) return;
	isOpen.value = !isOpen.value;
};

onMounted(() =>
{
	if (props.attachListener !== false)
	{
		const triggerElement = document.getElementById(props.elementId);
		if (triggerElement)
		{
			triggerElement.addEventListener("click", toggleMenu);
		}
		else
		{
			// console.error(`Menu trigger element with ID "${props.elementId}" not found.`);
		}
	}
});

const closeMenu = () =>
{
	isOpen.value = false;
};

const handleOutsideClick = (event: MouseEvent) =>
{
	const triggerElement = document.getElementById(props.elementId);
	// Close the menu if the click is outside of the menu itself and not on the trigger.
	if (isOpen.value && menuRef.value)
	{
		const path = event.composedPath();
		if (!path.includes(menuRef.value) && (!triggerElement || !path.includes(triggerElement)))
		{
			closeMenu();
		}
	}
};

const handleEscapeKey = (event: KeyboardEvent) =>
{
	if (event.key === "Escape")
		closeMenu();
};

// Add and remove global event listeners only when the menu is open for better performance.
watch(isOpen, async (isNowOpen) =>
{
	if (isNowOpen)
	{
		const triggerElement = document.getElementById(props.elementId);
		if (triggerElement)
		{
			const triggerRect = triggerElement.getBoundingClientRect();

			if (props.matchWidth)
			{
				menuWidth.value = `${triggerRect.width}px`;
			}
			else
			{
				menuWidth.value = undefined;
			}

			// Set a temporary position to allow the menu to render so we can get its dimensions.
			menuTop.value = triggerRect.bottom;
			menuLeft.value = triggerRect.left;

			// Wait for the DOM to update with the menu rendered.
			await nextTick();

			if (menuRef.value)
			{
				const menuRect = menuRef.value.getBoundingClientRect();
				const viewportWidth = window.innerWidth;
				const viewportHeight = window.innerHeight;

				let finalTop = triggerRect.bottom;
				let finalLeft = triggerRect.left;

				// Adjust if it overflows on the right
				if (finalLeft + menuRect.width > viewportWidth - 8)
				{
					finalLeft = triggerRect.right - menuRect.width;
				}

				// Hard clamp to ensure it doesn't overflow the right edge
				if (finalLeft + menuRect.width > viewportWidth - 8)
				{
					finalLeft = viewportWidth - menuRect.width - 8;
				}

				// Adjust if it overflows on the bottom, try placing it above the trigger
				if (finalTop + menuRect.height > viewportHeight)
				{
					finalTop = triggerRect.top - menuRect.height;
				}

				// Clamp to viewport edges as a final safeguard
				if (finalLeft < 8)
				{
					finalLeft = 8; // 8px padding from the edge
				}
				if (finalTop < 0)
				{
					finalTop = 8; // 8px padding from the edge
				}

				menuTop.value = finalTop;
				menuLeft.value = finalLeft;
			}
		}
		else
		{
			console.error(`Menu trigger element with ID "${props.elementId}" not found.`);
			closeMenu();
			return;
		}

		document.addEventListener("click", handleOutsideClick, true);
		document.addEventListener("keydown", handleEscapeKey);
	}
	else
	{
		document.removeEventListener("click", handleOutsideClick, true);
		document.removeEventListener("keydown", handleEscapeKey);
	}
}, { immediate: true });

onUnmounted(() =>
{
	const triggerElement = document.getElementById(props.elementId);
	if (triggerElement)
	{
		triggerElement.removeEventListener("click", toggleMenu);
	}
	// Cleanup listeners when the component is unmounted
	document.removeEventListener("click", handleOutsideClick, true);
	document.removeEventListener("keydown", handleEscapeKey);
});
</script>

<template>
	<!-- The menu is only in the DOM when it's open -->
	<div
		v-if="isOpen"
		ref="menuRef"
		class="menu-content"
		:style="{
			top: `${menuTop}px`,
			left: `${menuLeft}px`,
			width: menuWidth
		}"
		@click="closeMenu"
	>
		<slot></slot>
	</div>
</template>

<style scoped>
.menu-content {
	position: fixed; /* Use fixed positioning to place relative to viewport */

	/* Basic menu styling. Customize as needed. */
	background: var(--surface);
	color: var(--onsurface);
	border: 1px solid #e2e8f0;
	box-shadow: 0 4px 6px -1px rgb(0 0 0 / 10%), 0 2px 4px -2px rgb(0 0 0 / 10%);
	z-index: 50;
	min-width: 10rem; /* 160px */
}
</style>
