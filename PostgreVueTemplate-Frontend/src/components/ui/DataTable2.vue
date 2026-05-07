<script setup lang="ts">
import { ref, computed } from "vue";

interface Header {
	text: string;
	value: string;
}

const props = defineProps({
	headers: {
		type: Array as () => Header[],
		required: true,
	},
	items: {
		type: Array as () => any[],
		default: () => [],
	},
});

const sortBy = ref<string>("");
const sortDesc = ref(false);

const sortedItems = computed(() =>
{
	if (!props.items || props.items.length === 0)
	{
		return [];
	}

	const itemsCopy = [...props.items];
	if (sortBy.value)
	{
		itemsCopy.sort((a, b) =>
		{
			let valA = a[sortBy.value];
			let valB = b[sortBy.value];

			if (valA < valB)
			{
				return sortDesc.value ? 1 : -1;
			}
			if (valA > valB)
			{
				return sortDesc.value ? -1 : 1;
			}
			return 0;
		});
	}
	return itemsCopy;
});

function sort(key: string)
{
	if (sortBy.value === key)
	{
		sortDesc.value = !sortDesc.value;
	}
	else
	{
		sortBy.value = key;
		sortDesc.value = false;
	}
}
</script>

<template>
	<div overflow-x-auto>
		<table min-w-full >
			<thead bg-surface3>
				<tr>
					<th
						v-for="header in headers"
						:key="header.value"
						scope="col"
						cursor-pointer px-6 py-3 text-left text-xs font-medium tracking-wider uppercase
						@click="sort(header.value)"
					>
						<div flex items-center gap-2>
							{{ header.text }}
							<div v-if="sortBy === header.value">
								<div :class="sortDesc ? 'i-mdi:arrow-down' : 'i-mdi:arrow-up'" />
							</div>
						</div>
					</th>
				</tr>
			</thead>
			<tbody bg-surface divide-y divide-gray-700>
				<tr v-for="(item, index) in sortedItems" :key="index" hover:bg-surfacehighlight >
					<td v-for="header in headers" :key="header.value" p-2>
						<slot :name="`item.${header.value}`" :item="item">
							{{ item[header.value] }}
						</slot>
					</td>
				</tr>
			</tbody>
		</table>
	</div>
</template>