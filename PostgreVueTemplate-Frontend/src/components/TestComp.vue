<script setup lang="ts">
import { ref } from "vue";

const lol: number = 123;
const hihi = ref();

// const hihi:number = 123;


function handleClick()
{
	console.log("hihi");
}

</script>

<template>
	<i-carbon-accessibility />
	{{ $t('LangName') }}
	<button mt-3 @click="handleClick">
		hihi
	</button>
</template>

<style scoped>
.hihi {
	background-color: red;
	mask-image: var(--svg);
}

</style>