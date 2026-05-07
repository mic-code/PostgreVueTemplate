<script setup lang="ts">
import Icon from "./Icon.vue";

defineProps({
	level: Number,
	icon: String

});

</script>

<template>
	<div mb-3 flex gap-3 >
		<Icon :icon="icon" />
		<p>
			<slot/>
		</p>
	</div>
</template>
