<script setup lang="ts">
import { computed } from "vue";

const props = defineProps({
	icon: { type: String, default: "i-clarity:warning-standard-solid" },
	text: String,
	type: String
});

const onColor = computed(()=>
{
	return props.type != null;
});

</script>

<template>
	<div class="alert" :class="`bg-${type}`" flex pa-2 r-2 >
		<Icon my-a :icon="icon" :color="onColor?'oncolor':'onsurface'"/>
		<p ms-2 flex :class="`${onColor?'text-oncolor':'text-onsurface'}`">
			{{ text }}
			<slot/>
		</p>
	</div>
</template>

<style>
.alert {
	box-shadow: 0 2px 4px rgb(0 0 0 / 10%);
}
</style>