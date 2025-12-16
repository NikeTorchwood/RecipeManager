<script setup lang="ts">
import RecipeCard from './RecipeCard.vue'
import type { Recipe } from '../models/Recipe'

defineProps<{ recipeList: Recipe[] }>()

const emit = defineEmits<{
    (e: 'edit', recipe: Recipe): void
    (e: 'delete', id: string): void
}>()

const onEdit = (recipe: Recipe) => emit('edit', recipe)
const onDelete = (id: string) => emit('delete', id)
</script>

<template>
    <el-row :gutter="20">
        <el-col v-for="recipe in recipeList" :key="recipe.id" :xs="24" :sm="12" :md="8" :lg="6">
            <RecipeCard :recipe="recipe" @edit="onEdit" @delete="onDelete" />
        </el-col>
    </el-row>
</template>
