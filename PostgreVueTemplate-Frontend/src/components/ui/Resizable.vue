<script setup lang="ts">
import { onMounted, ref } from "vue";

defineProps({
	allowResize: { type: Boolean, default: true },
	fill: { type: Boolean, default: true }
});

const widthModel = defineModel<number>("width");
const heightModel = defineModel<number>("height");

const container = ref<HTMLElement>();

function adjustFontSize(width: number)
{
	const fontSize = width * 0.1;
	if (container.value)
		container.value.style.fontSize = `${fontSize}px`;
}

const resizeObserver = new ResizeObserver(entries =>
{
	for (const entry of entries)
	{
		const { width, height } = entry.contentRect;
		widthModel.value = width;
		heightModel.value = height;
		adjustFontSize(width);
	}
});

onMounted(() =>
{
	widthModel.value = container.value.offsetWidth;
	heightModel.value = container.value.offsetHeight;

	resizeObserver.observe(container.value);
	adjustFontSize(container.value.offsetWidth);
});

</script>

<template>
	<div ref="container" class="resizableCon" :class="[allowResize ? 'resize' : '', fill?'fill':'']">
		<slot />
	</div>
</template>

<style scoped>
.fill {
	width: 100%;
	height: 100%;
}

.resizableCon {
	/* overflow: hidden; */

	/* overflow: scroll; */
	overflow: auto;
	background-color: white;
	box-shadow: 0 4px 8px rgb(0 0 0 / 10%);
}

.resizableCon :deep(h1) {
	font-size: 2em;
	line-height: 1em;
}

.resize {
	resize: both;
}
</style>