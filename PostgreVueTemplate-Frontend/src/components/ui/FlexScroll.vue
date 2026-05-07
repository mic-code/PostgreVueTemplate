<script setup lang="ts">
defineProps({
	row: { type: Boolean, default: false },
});

</script>

<template>
	<div v-if="row" flex-1 overflow-x-auto grid>
		<div min-w-0>
			<slot/>
		</div>
	</div>

	<div v-else flex-1 overflow-y-auto>
		<div h-0>
			<slot/>
		</div>
	</div>
</template>