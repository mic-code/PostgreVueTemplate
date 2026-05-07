<script setup lang="ts">
import { poiIcons } from "../../utilities/iconUtility";

defineProps<{
	modelValue?: string | null;
	defaultIcon: string;
}>();

const emit = defineEmits<{
	(e: "change", icon: { name: string; url: string }): void;
}>();

const uniqueId = `icon-selector-${Math.random().toString(36).substr(2, 9)}`;

function selectIcon(icon: typeof poiIcons[0])
{
	emit("change", icon);
}
</script>

<template>
	<div>
		<div :id="uniqueId" class="cursor-pointer" pointer-events-auto>
			<img v-if="modelValue" :src="modelValue" style="width: 1.5rem; height: 1.5rem;" alt="icon">
			<Icon v-else size="1.5rem" :icon="defaultIcon"></Icon>
		</div>

		<Menu :element-id="uniqueId">
			<div pointer-events-auto flex flex-col class="max-h-96 overflow-y-auto rounded bg-surface2 p-2">
				<div
					v-for="i in poiIcons"
					:key="i.name"
					class="flex cursor-pointer items-center gap-2 p-2 hover:bg-primary/20"
					@click="selectIcon(i)"
				>
					<img :src="i.url" class="h-6 w-6" :alt="i.name">
					<span class="text-sm">{{ i.name }}</span>
				</div>
			</div>
		</Menu>
	</div>
</template>
