# TetriX - Classic Remake

Un clon del clásico **Tetris** desarrollado en Unity, enfocado en mecánicas fluidas y sistemas de juego modernos como piezas fantasma, gestión de combos y dificultad progresiva.

## 🚀 Características Principales

* **Mecánicas Clásicas:** Movimiento lateral, rotación y sistema de "Hard Drop" para una jugabilidad ágil.
* **Sistema de Sombra (Ghost Piece):** Visualización en tiempo real de la posición de aterrizaje de la pieza, respetando el grid y la rotación actual.
* **Gestión de Combos Persistente:** Multiplicador de puntos acumulativo que aumenta mientras limpies filas con piezas consecutivas.
* **Dificultad Progresiva:** La velocidad de caída se ajusta automáticamente cada 1000 puntos basándose en el nivel del jugador.
* **Sistema de Basura (Garbage Lines):** Generación automática de filas incompletas cada 10 segundos para aumentar el desafío.
* **Control DAS (Delayed Auto Shift):** Movimiento continuo al mantener pulsadas las teclas para evitar el cansancio por clics repetitivos.

## 🛠️ Detalles Técnicos

* **Motor:** Unity 2022.3.20f1.
* **Lógica de Grid:** Matriz de `Transform` de 10x20 que gestiona la ocupación y limpieza de filas.
* **UI:** Integración con **TextMeshPro** para visualización de Score y animaciones de Combo.

## 🎮 Controles

| Tecla | Acción |
| :--- | :--- |
| **Flechas Izquierda / Derecha** | Movimiento lateral (Manten pulsado para movimiento continuo). |
| **Flecha Arriba** | Rotar pieza 90°. |
| **Flecha Abajo** | Caída rápida (Soft Drop). |
| **Espacio** | Caída instantánea (Hard Drop). |

## 💡 Créditos e Inspiración

Este proyecto ha sido desarrollado tomando como base lógica inicial y punto de inspiración el trabajo de **Marvin Paul**.
* **Repositorio de referencia:** [marvpaul en GitHub](https://github.com/marvpaul)

---
