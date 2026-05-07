<script setup lang="ts">
import { ref, computed, watch } from "vue";

interface TreeData {
	Id: string;
	Name: string;
	IsVisible: boolean;
	children?: TreeData[];
}

const props = defineProps({
	tree: Object,
	selectedId: String,
	size: { type: String, default: "1.5rem" },
	showRoot: Boolean,
	searchQuery: { type: String, default: "" }
});

const emit = defineEmits<{
	onSelect: [ target: object],
	onDoubleClick: [ target: object]
}>();

const typedTree = computed(() => props.tree as TreeData | undefined);

// Collapse by default except for root (when showRoot is false)
const isOpen = ref(!props.showRoot);

const hasChildren = computed(() =>
{
	return typedTree.value && typedTree.value.children && typedTree.value.children.length > 0;
});

// Auto-expand when there's a search query and node has matching children
watch([() => props.searchQuery, typedTree], ([query, tree]) =>
{
	if(query && query.trim() && tree?.children?.length)
	{
		// Check if this node has children (matching results)
		if(tree.children.length > 0)
		{
			isOpen.value = true;
		}
	}
}, { immediate: true });

function toggle()
{
	if (hasChildren.value)
		isOpen.value = !isOpen.value;
}

function handleSelect(target)
{
	emit("onSelect", target);
}

function handleDoubleClick(target)
{
	emit("onDoubleClick", target);
}

const isSelected = computed(()=>
{
	return typedTree.value.Id && typedTree.value.Id === props.selectedId;
});

</script>

<template>
	<div v-if="typedTree">
		<div
			v-if="showRoot"
			class="node-item"
			:class="{ 'selected': isSelected }"
			@click="handleSelect(typedTree)"
			@dblclick="handleDoubleClick(typedTree)">
			<Icon :size="size" v-if="hasChildren && isOpen" icon="i-material-symbols:arrow-drop-down" @click.stop="toggle"/>
			<Icon :size="size" v-if="hasChildren && !isOpen" icon="i-material-symbols:arrow-right" @click.stop="toggle"/>
			<Icon :size="size" v-else-if="!hasChildren && !isSelected" icon="i-radix-icons:dot"/>
			<Icon :size="size" v-else-if="isSelected" icon="i-radix-icons:dot-filled"/>
			<span style="overflow-wrap: anywhere;" >{{ typedTree.Name }}</span>
			<slot :node="typedTree" :isSelected="isSelected"></slot>
		</div>
		<div id="children" v-if="hasChildren && isOpen" class="child-nodes" :class="showRoot? 'padLevel':''">
			<div v-for="(child, index) in typedTree.children" :key="index" >
				<TreeView :show-root="true" :tree="child" :selectedId="selectedId" :search-query="searchQuery" @on-select="handleSelect" @on-double-click="handleDoubleClick">
					<template #default="{ node, isSelected }">
						<slot :node="node" :isSelected="isSelected"></slot>
					</template>
				</TreeView>
			</div>
		</div>
	</div>
</template>

<style scoped>
.node-item {
	cursor: pointer;
	display: flex;
	align-items: center;
	user-select: none;
	color: var(--onsurface);

	/* background-color: rgb(from var(--surface) r g b / 60%); */
}

.node-item:hover {
	background-color: rgb(from var(--surface) r g b / 80%);
}

.node-item.selected {
	background-color: var(--secondary);
	color: var(--oncolor);
}

.toggle {
	width: 1.5em;
	display: inline-block;
	text-align: center;
}

.no-toggle {
	width: 1.5em;
	display: inline-block;
}

.child-nodes {
	list-style-type: none;
	margin: 0;
	overflow-y: auto;
}

.padLevel {
	padding-left: 1.5rem;
}
</style>
