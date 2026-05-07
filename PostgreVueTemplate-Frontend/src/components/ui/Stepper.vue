<script setup lang="ts">
import { ProgressState } from "../../utilities/progressUtility";


interface StepData {
	name: string;
	progress: number;
	error: string,
	state: string
}

const props = defineProps({
	steps: { type: Array<StepData>, default: null },
	size: { type: String, default: "2rem" }
});
</script>

<template>
	<div flex flex-col>
		<div v-for="(step, index) in steps" :key="step.name" flex>
			<div mr-4 flex flex-col items-center :style="`min-height: calc(${size} * 2);`" >
				<Icon v-if="step.state == ProgressState.Waiting" :size="size" icon="i-pajamas:status-waiting" text-secondary/>
				<Icon v-if="step.state == ProgressState.InProgress" :size="size" icon="i-svg-spinners:180-ring-with-bg" text-onsurface/>
				<Icon v-if="step.state == ProgressState.Done" :size="size" icon="i-icomoon-free:checkbox-checked" text-success/>
				<Icon v-if="step.state == ProgressState.Failed" :size="size" icon="i-clarity:warning-standard-solid" text-error/>
				<div v-if="index < steps.length - 1" :style="`width:calc(${size} / 10);`" flex-grow bg-secondary />
			</div>
			<div :style="`font-size: calc(${size} / 2);`">
				<p my-0 class="flex items-center" :style="{ height: size }" :class="step.state == ProgressState.InProgress?'font-bold text-onsurface':'text-onsurface2'">
					{{ $t(step.name) }}
				</p>
				<p v-if="step.state == ProgressState.InProgress" my-0 class="h-50%" text-onsurface font-bold>
					{{ (step.progress*100).toFixed(0) }}%
				</p>
				<p v-if="step.state == ProgressState.Failed" class="h-50%" text-error my-0 >
					{{ step.error }}
				</p>
			</div>
		</div>
	</div>
</template>
