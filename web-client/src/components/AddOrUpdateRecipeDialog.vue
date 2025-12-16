<script setup lang="ts">
import { ref, computed, watch, reactive } from "vue";
import type { Recipe } from '../models/Recipe';
import type { FormInstance, FormRules } from 'element-plus'

const emit = defineEmits<{
    (event: 'update:visible', val: boolean): void
    (event: 'submit', recipe: Recipe): void
}>()

const props = defineProps<{
    visible: boolean
    recipe?: Recipe | null
}>()

const formRef = ref<FormInstance>()

const form = reactive({
    title: '',
    description: '',
    categoryName: ''
})

const rules: FormRules = {
    title: [{ required: true, message: 'Введите название', trigger: 'blur' }],
    categoryName: [{ required: true, message: 'Введите категорию', trigger: 'blur' }]
}

const isEdit = computed(() => !!props.recipe)

const dialogVisible = computed({
    get: () => props.visible,
    set: (val) => emit('update:visible', val)
})

watch(
    () => props.visible,
    (isOpen) => {
        if (isOpen) {
            if (props.recipe) {
                form.title = props.recipe.title
            } else {
                form.title = ''
                setTimeout(() => formRef.value?.resetFields(), 0)
            }
        }
    }
)

const close = () => {
    dialogVisible.value = false;
}

const onCancel = () => close()

const onAdd = async () => {
    if (!formRef.value) return;

    await formRef.value.validate(async (valid) => {
        if (valid) {
            const newRecipe: Recipe = {
                id: props.recipe?.id ?? crypto.randomUUID(),

                title: form.title,
                description: form.description,
            }

            emit("submit", newRecipe)
            close();
        }
    })
}
</script>

<template>
    <el-dialog :title="isEdit ? 'Редактировать рецепт' : 'Добавить рецепт'" v-model="dialogVisible" width="500px"
        destroy-on-close>

        <el-form label-width="120px" ref="formRef" :model="form" :rules="rules" @submit.prevent="onAdd"
            @keydown.enter.stop.prevent="onAdd">
            <el-form-item label="Название" prop="title">
                <el-input v-model="form.title" placeholder="Название рецепта" />
            </el-form-item>

            <el-form-item label="Описание" prop="description">
                <el-input v-model="form.description" placeholder="Описание" />
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