<script setup lang="ts">
const emit = defineEmits<{
    (e: 'add-clicked'): void
}>();

defineProps<{
    recipeCount: number
}>()

const handleAddClick = (event: Event) => {
    const target = event.currentTarget as HTMLElement;
    target?.blur();

    emit('add-clicked');
}
</script>

<template>
    <el-page-header :icon="null">
        <template #title>Рецепты</template>
        <template #content>
            <span class="text-large font-600 mr-3"> Всего: {{ recipeCount }} </span>
        </template>
        <template #extra>
            <div class="flex items-center">
                <el-button type="primary" @click="handleAddClick">Добавить рецепт</el-button>
            </div>
        </template>
    </el-page-header>
</template>