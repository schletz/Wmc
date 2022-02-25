<template>
    <header>Store Manager</header>
    <main>
        <nav class="sidebar">
            <ul data-nav-group="Stores verwalten">
                <li><i class="fas fa-link"></i><router-link to="/stores">Stores</router-link></li>
            </ul>
            <ul data-nav-group="Produkte verwalten">
                <li><i class="fas fa-link"></i><router-link to="/">Go to Home</router-link></li>
            </ul>
        </nav>
        <div class="main-content">
            <router-view></router-view>
        </div>
    </main>
    <footer>
        <img src="img/schullogo.png" alt="Schullogo" style="height: 32px" class="me-3" />
        <strong class="pe-3"> HTL Spengergasse - Abteilung Informatik </strong>
        &copy; Michael Schletz, 2022
    </footer>
</template>

<script>


export default {
    name: 'App',
    components: {

    },
};
</script>

<style>
:root {
    --sidebar-background-color: hsl(0, 0%, 98%);
    --sidebar-separator-color: hsl(300, 25%, 70%);
    --sidebar-groupheader-color: hsl(300, 25%, 50%);
    --sidebar-item-color: hsl(0, 0%, 0%);
    --sidebar-item-background-color: hsl(0, 0%, 90%);
}

html,
body,
#app {
    width: 100%;
    height: 100%;
    padding: 0;
    margin: 0;
}

#app {
    display: flex;
    flex-direction: column;
}

header {
    display: flex;
    align-items: center;
    flex: 0 0 70px;
    border-bottom: 1px solid var(--sidebar-separator-color);
    padding: 0rem 1rem;
}

.sidebar {
    background-color: var(--sidebar-background-color);
    padding-top: 1rem;
    padding-right: 1rem;
    flex: 0 0 200px;
    overflow-y: auto;
}

.sidebar ul {
    list-style: none;
    padding: 0;
    margin: 0;
}

.sidebar li {
    padding-left: 1rem;
}

.sidebar a {
    text-decoration: none;
    color: inherit;
}

ul[data-nav-group] {
    margin-top: 1rem;
    padding-left: 0rem;
    border-top: 1px solid var(--sidebar-separator-color);
}

ul[data-nav-group]:first-child {
    border: none;
    margin-top: 0.5rem;
}

ul[data-nav-group]::before {
    content: attr(data-nav-group);
    text-transform: uppercase;
    color: var(--sidebar-groupheader-color);
    font-weight: bolder;
    line-height: 1.1em;
    display: block;
    margin-bottom: 0.5rem;
    padding-left: 1rem;
}

main {
    display: flex;
    flex-grow: 1;
    align-items: stretch;
    overflow-y: hidden;
}

.main-content {
    padding-left: 1rem;
    padding-right: 1rem;
    margin-top: 1rem;
    flex-grow: 1;
    overflow-y: auto;
}

footer {
    border-top: 1px solid var(--sidebar-separator-color);
    padding: 1rem 1rem;
}

.card {
    background-color: hsl(0, 0%, 97%);
    margin-bottom: 1rem;
}
</style>
