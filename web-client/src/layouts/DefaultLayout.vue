<template>
    <el-container>
        <el-header>
            <slot name="header" />
        </el-header>

        <el-main>
            <slot name="content" />
        </el-main>
    </el-container>
</template>
