<script setup lang="ts">
import { Vec2 } from "../../utilities/vec2";

const props = defineProps({
	modelValue: { type: Vec2, default: { x: 450, y: 70 } }
});

const emit = defineEmits(["update:modelValue"]);

function onMouseDown(event: MouseEvent)
{
	const draggableDiv = event.currentTarget as HTMLElement;
	const originalTarget = event.target as HTMLElement;

	// Do not drag if the click is on a fixed position element inside the draggable
	let p: HTMLElement | null = originalTarget;
	while (p && p !== draggableDiv)
	{
		if (window.getComputedStyle(p).position === "fixed")
		{
			return;
		}
		p = p.parentElement;
	}

	let target = originalTarget;
	let depth = 0;

	while (target && target !== draggableDiv)
	{
		target = target.parentElement;
		depth++;
	}

	const handle = originalTarget.closest(".draggable");
	const canBeDragged = handle && draggableDiv.contains(handle);

	if (depth >= 2 && !canBeDragged)
		return;

	event.preventDefault();

	const startX = event.clientX;
	const startY = event.clientY;
	const startLeft = props.modelValue.x;
	const startTop = props.modelValue.y;

	function onMouseMove(moveEvent: MouseEvent)
	{
		const dx = moveEvent.clientX - startX;
		const dy = moveEvent.clientY - startY;
		emit("update:modelValue", { x: startLeft + dx, y: startTop + dy });
	}

	function onMouseUp()
	{
		document.removeEventListener("mousemove", onMouseMove);
		document.removeEventListener("mouseup", onMouseUp);
	}

	document.addEventListener("mousemove", onMouseMove);
	document.addEventListener("mouseup", onMouseUp);
}
</script>

<template>
	<div
		class="absolute cursor-move"
		:style="{ left: `${props.modelValue.x}px`, top: `${props.modelValue.y}px` }"
		@mousedown="onMouseDown"
	>
		<slot/>
	</div>
</template>
