<script setup lang="ts">
import { ref, computed, watch } from "vue";
import type { Recipe } from '../models/Recipe';
import type { FormInstance, FormRules } from 'element-plus'

const emit = defineEmits<{
    (event: 'update:visible', val: boolean): void
    (event: 'submit', recipe: Recipe): void
}>()

const props = defineProps<{
    visible: boolean;
    recipe?: Recipe | null;
    categories: any[];
}>()

const formRef = ref<FormInstance>()

const form = ref({
    title: '',
    description: '',
    categoryId: ''
});

const rules: FormRules = {
    title: [{ required: true, message: 'Введите название', trigger: 'blur' }],
    categoryId: [{ required: true, message: 'Выберите категорию', trigger: 'change' }]
}

const isEdit = computed(() => !!props.recipe)

const dialogVisible = computed({
    get: () => props.visible,
    set: (val) => emit('update:visible', val)
})

watch(() => props.recipe, (newVal) => {
    if (newVal) {
        form.value = {
            title: newVal.title,
            description: newVal.description,
            categoryId: newVal.categoryId || ''
        };
    } else {
        form.value = { title: '', description: '', categoryId: '' };
    }
}, { immediate: true });

const close = () => {
    dialogVisible.value = false;
    formRef.value?.resetFields();
}

const onCancel = () => close()

const onAdd = async () => {
    if (!formRef.value) return;

    await formRef.value.validate((valid) => {
        if (valid) {
            const newRecipe: Recipe = {
                id: props.recipe?.id ?? crypto.randomUUID(),

                title: form.value.title,
                description: form.value.description,
                categoryId: form.value.categoryId
            }

            emit("submit", newRecipe)
        }
    })
}
</script>

<template>
    <el-dialog :title="isEdit ? 'Редактировать рецепт' : 'Добавить рецепт'" v-model="dialogVisible" width="500px"
        @close="onCancel">

        <el-form label-width="120px" ref="formRef" :model="form" :rules="rules" @submit.prevent="onAdd">
            <el-form-item label="Название" prop="title">
                <el-input v-model="form.title" placeholder="Название рецепта" />
            </el-form-item>

            <el-form-item label="Категория" prop="categoryId">
                <el-select v-model="form.categoryId" placeholder="Выберите категорию" style="width: 100%">
                    <el-option v-for="item in categories" :key="item.id" :label="item.name" :value="item.id" />
                </el-select>
            </el-form-item>

            <el-form-item label="Описание" prop="description">
                <el-input v-model="form.description" placeholder="Описание" type="textarea" :rows="3" />
            </el-form-item>
        </el-form>

        <template #footer>
            <el-button @click="onCancel">Отмена</el-button>
            <el-button type="primary" @click="onAdd">
                {{ isEdit ? 'Сохранить' : 'Добавить' }}
            </el-button>
        </template>
    </el-dialog>
</template>

<style scoped></style>