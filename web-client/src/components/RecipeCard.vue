<script setup lang="ts">
import type { Recipe } from '../models/Recipe'

const props = defineProps<{ recipe: Recipe }>()

const emit = defineEmits<{
    (e: 'edit', recipe: Recipe): void
    (e: 'delete', id: string): void
}>()
</script>

<template>
    <el-card shadow="hover" class="recipe-card">
        <template #header>
            <div class="card-header">
                <span class="card-title">{{ recipe.title }}</span>
            </div>
        </template>

        <div class="card-footer">
            <el-button size="small" type="primary" @click="emit('edit', recipe)">Ред.</el-button>
            <el-button size="small" type="danger" @click="emit('delete', recipe.id)">Удалить</el-button>
        </div>
    </el-card>
</template>

<style scoped>
.recipe-card {
    height: 100%;
    display: flex;
    flex-direction: column;
}

:deep(.el-card__body) {
    flex: 1;
    display: flex;
    flex-direction: column;
}

.card-header {
    font-weight: 600;
    font-size: 16px;
}

.card-body {
    flex: 1;
}

.mb-2 {
    margin-bottom: 8px;
    display: inline-block;
}

.card-description {
    font-size: 14px;
    margin-bottom: 12px;
    word-break: break-word;
    color: #606266;
}

.card-date {
    font-size: 12px;
    color: #909399;
    margin-top: auto;
    margin-bottom: 10px;
}

.card-footer {
    display: flex;
    justify-content: flex-end;
    gap: 8px;
    margin-top: 10px;
    border-top: 1px solid #ebeef5;
    padding-top: 10px;
}
</style>