<script setup lang="ts">

import { ref, watch, onMounted } from "vue";

const color = defineModel<string>("color", { default: "#FFFFFF" });
const emit = defineEmits(["editEnded"]);

const props = defineProps<{
	disabled?: boolean;
}>();

// Refs for canvas elements
const mainCanvas = ref<HTMLCanvasElement | null>(null);
const hueCanvas = ref<HTMLCanvasElement | null>(null);

// Color state in HSL
const hue = ref(0);
const saturation = ref(100);
const lightness = ref(50);

// #region Color Conversion
function hslToRgb(h: number, s: number, l: number): [number, number, number]
{
	s /= 100;
	l /= 100;
	const k = (n: number) => (n + h / 30) % 12;
	const a = s * Math.min(l, 1 - l);
	const f = (n: number) => l - a * Math.max(-1, Math.min(k(n) - 3, 9 - k(n), 1));
	return [255 * f(0), 255 * f(8), 255 * f(4)];
}

function rgbToHex(r: number, g: number, b: number): string
{
	return "#" + [r, g, b].map(x =>
	{
		const hex = Math.round(x).toString(16);
		return hex.length === 1 ? "0" + hex : hex;
	}).join("");
}

function hexToHsl(hex: string): { h: number, s: number, l: number }
{
	// Convert hex to RGB first
	let r = 0, g = 0, b = 0;
	if (hex.length === 4)
	{
		r = parseInt(hex[1] + hex[1], 16);
		g = parseInt(hex[2] + hex[2], 16);
		b = parseInt(hex[3] + hex[3], 16);
	}
	else if (hex.length === 7)
	{
		r = parseInt(hex[1] + hex[2], 16);
		g = parseInt(hex[3] + hex[4], 16);
		b = parseInt(hex[5] + hex[6], 16);
	}
	r /= 255;
	g /= 255;
	b /= 255;
	const max = Math.max(r, g, b);
	const min = Math.min(r, g, b);
	let h = 0, s = 0;
	let l = (max + min) / 2;

	if (max !== min)
	{
		const d = max - min;
		s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
		switch (max)
		{
			case r: h = (g - b) / d + (g < b ? 6 : 0); break;
			case g: h = (b - r) / d + 2; break;
			case b: h = (r - g) / d + 4; break;
		}
		h /= 6;
	}

	return { h: h * 360, s: s * 100, l: l * 100 };
}
// #endregion

// #region Canvas Drawing
function drawMainCanvas()
{
	if (!mainCanvas.value) return;
	const ctx = mainCanvas.value.getContext("2d");
	if (!ctx) return;

	const width = mainCanvas.value.width;
	const height = mainCanvas.value.height;

	for (let y = 0; y < height; y++)
	{
		const l = 100 - (y / (height - 1)) * 100;
		const grad = ctx.createLinearGradient(0, 0, width, 0);
		grad.addColorStop(0, `hsl(${hue.value}, 0%, ${l}%)`);
		grad.addColorStop(1, `hsl(${hue.value}, 100%, ${l}%)`);
		ctx.fillStyle = grad;
		ctx.fillRect(0, y, width, 1);
	}
}

function drawHueCanvas()
{
	if (!hueCanvas.value) return;
	const ctx = hueCanvas.value.getContext("2d");
	if (!ctx) return;

	const width = hueCanvas.value.width;
	const height = hueCanvas.value.height;

	const gradient = ctx.createLinearGradient(0, 0, 0, height);
	for (let i = 0; i <= 360; i += 60)
	{
		gradient.addColorStop(i / 360, `hsl(${i}, 100%, 50%)`);
	}
	ctx.fillStyle = gradient;
	ctx.fillRect(0, 0, width, height);
}
// #endregion

function updateColorFromHsl()
{
	const [r, g, b] = hslToRgb(hue.value, saturation.value, lightness.value);
	color.value = rgbToHex(r, g, b);
}

watch(color, (newColor) =>
{
	const { h, s, l } = hexToHsl(newColor);
	hue.value = h;
	saturation.value = s;
	lightness.value = l;
	drawMainCanvas();
}, { immediate: true });

watch(hue, () =>
{
	drawMainCanvas();
	updateColorFromHsl();
});

onMounted(() =>
{
	drawMainCanvas();
	drawHueCanvas();
});

// #region Mouse Events
let isDraggingMain = false;
let isDraggingHue = false;

function handleMainMouseDown(e: MouseEvent)
{
	if (props.disabled) return;
	isDraggingMain = true;
	updateMainColor(e);
	window.addEventListener("mousemove", handleMainMouseMove);
	window.addEventListener("mouseup", handleMainMouseUp);
}

function handleMainMouseMove(e: MouseEvent)
{
	if (isDraggingMain)
	{
		updateMainColor(e);
	}
}

function handleMainMouseUp()
{
	isDraggingMain = false;
	window.removeEventListener("mousemove", handleMainMouseMove);
	window.removeEventListener("mouseup", handleMainMouseUp);
	emit("editEnded", color);
}

function updateMainColor(e: MouseEvent)
{
	if (!mainCanvas.value) return;
	const rect = mainCanvas.value.getBoundingClientRect();
	const x = Math.max(0, Math.min(rect.width, e.clientX - rect.left));
	const y = Math.max(0, Math.min(rect.height, e.clientY - rect.top));

	saturation.value = (x / rect.width) * 100;
	lightness.value = 100 - (y / rect.height) * 100;

	updateColorFromHsl();
}

function handleHueMouseDown(e: MouseEvent)
{
	if (props.disabled) return;
	isDraggingHue = true;
	updateHue(e);
	window.addEventListener("mousemove", handleHueMouseMove);
	window.addEventListener("mouseup", handleHueMouseUp);
}

function handleHueMouseMove(e: MouseEvent)
{
	if (isDraggingHue)
	{
		updateHue(e);
	}
}

function handleHueMouseUp()
{
	isDraggingHue = false;
	window.removeEventListener("mousemove", handleHueMouseMove);
	window.removeEventListener("mouseup", handleHueMouseUp);
	emit("editEnded", color);
}

function updateHue(e: MouseEvent)
{
	if (!hueCanvas.value) return;
	const rect = hueCanvas.value.getBoundingClientRect();
	const y = Math.max(0, Math.min(rect.height, e.clientY - rect.top));

	hue.value = (y / rect.height) * 360;
}
// #endregion

</script>

<template>
	<div class="gradient-picker" :class="{ 'opacity-50 pointer-events-none': disabled }">
		<div class="main-picker">
			<canvas ref="mainCanvas" width="200" height="150" @mousedown="handleMainMouseDown"></canvas>
			<div
				class="main-picker-cursor"
				:style="{ left: `${saturation}%`, top: `${100 - lightness}%` }"
			></div>
		</div>
		<div class="hue-slider">
			<canvas ref="hueCanvas" width="20" height="150" @mousedown="handleHueMouseDown"></canvas>
			<div class="hue-slider-cursor" :style="{ top: `${hue / 3.6}%` }"></div>
		</div>
	</div>
</template>

<style scoped>
.gradient-picker {
	display: flex;
	padding: 0.5rem;
	gap: 0.5rem;
	user-select: none;
}

.main-picker {
	position: relative;
	cursor: crosshair;
}

.main-picker-cursor {
	position: absolute;
	width: 10px;
	height: 10px;
	border-radius: 50%;
	border: 2px solid white;
	box-shadow: 0 0 0 1.5px black, inset 0 0 0 1.5px black;
	transform: translate(-50%, -50%);
	pointer-events: none; /* So it doesn't interfere with mouse events on canvas */
}

.hue-slider {
	position: relative;
	cursor: pointer;
}

.hue-slider-cursor {
	position: absolute;
	width: 100%;
	height: 4px;
	background: white;
	border: 1px solid black;
	transform: translateY(-50%);
	pointer-events: none;
	left: 0;
	box-sizing: border-box;
}

canvas {
	display: block;
	border-radius: 4px;
}
</style>
