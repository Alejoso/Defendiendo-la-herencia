# 🎃 Halloween Paisa: **La Finca a las 3:33 a. m.** 👺🌕🌲

> **Género:** 2D top-down (ortográfico)
> **Duración por partida:** 20–30 minutos  
> **Motor sugerido:** Unity 

![Captura principal](docs/hero.png)
*(Reemplaza esta imagen por un GIF o screenshot de tu juego.)*

---

## 🧭 Índice
- [Historia](#-historia)
- [Gameplay Loop](#-gameplay-loop)
- [Mecánicas de Locura](#-mecánicas-de-locura)
- [Mecánicas de Combate (Arma + Melee)](#️-mecánicas-de-combate-arma--melee)
- [Enemigos](#-enemigos)
- [Mapa y Áreas](#️-mapa-y-áreas)
- [Armas y Progresión](#-armas-y-progresión)
- [Objetivos y Condiciones](#-objetivos-y-condiciones)
- [Controles](#-controles)
- [Ajustes de Proyecto (Unity)](#-ajustes-de-proyecto-unity)
- [Balance Inicial](#-balance-inicial)
- [Estructura de Carpetas](#-estructura-de-carpetas)
- [Roadmap](#-roadmap)
- [Créditos](#-créditos)
- [Cómo jugar (rápido)](#-cómo-jugar-rápido)

---

## 🎭 Historia
Tu **abuelo** te cita en la finca **siempre a las 3:33 a. m.** para cuidar los terrenos: él salió a **pelear con el diablo**. Lo que parece una guardia normal se vuelve una noche de espantos. Si sobrevives a las oleadas y proteges cada zona, desbloquearás el **enfrentamiento final contra el Diablo** para **vengar al abuelo**.

---

## 🕹️ Gameplay Loop
1. **Explora** la finca (establo, cafetal, casa principal, lago, taller).  
2. **Defiende** el objetivo del área contra oleadas crecientes.  
3. **Gana recompensas** (armas y mejoras) al completar cada defensa.  
4. **Gestiona 3 barras**:
   - **Vida**: si llega a 0, pierdes.  
   - **Cordura** (Locura): sube con el terror; a más locura, más difícil.  
5. **Consume “paletas”** (dulces) para **curar Vida**.  
6. **Progresión de enemigos**: de **árboles** “vivos” y **arbustos** a bestias y leyendas.  
7. **Jefe final**: derrota al **Diablo** en el último sector durante la hora maldita.

---

## 😵‍💫 Mecánicas de Locura
- **Medidor de Locura**: sube con tiempo en sombras, gritos de leyendas, recibir daño y eventos.  

---

## 🗡️🔫 Mecánicas de Combate (Arma + Melee)
- **Loadout fijo:** el personaje siempre lleva **1 arma a distancia** y **1 arma cuerpo a cuerpo (melee)**.  
- **Agotamiento de munición:** cuando el cargador del arma a distancia llega a **0**, el personaje **cambia automáticamente a melee**.  
- **Bloqueo temporal a melee:** tras quedarse sin balas, queda **bloqueado en melee** por un periodo corto (recomendado **5–8 s**, configurable), **antes** de poder volver a disparar.  
- **Recuperación:** pasado el bloqueo, se puede volver a usar el arma de fuego.  
- **Ritmo de juego:** alterna entre **disparos precisos** para controlar oleadas y **ventanas de melee** para rematar, reposicionarte o conservar recursos.  

---

## 👹 Enemigos
**Comunes:** Árboles (raíces, empuje), Arbustos (camuflaje y ralentización), Lobos (flanqueo).  
**Leyendas:** **Patasola** (velocidad, dash y sangrado), **Llorona** (zonas de lamento que suben locura).  
**Élite:** **Diablito** (pre-jefe móvil, invoca adds).  
**Jefe:** **Diablo** (fases, lluvia de fuego y portales).

---

## 🗺️ Mapa y Áreas
Orden recomendado y recompensa principal:

1. **Establo** – inicio; desbloquea **Palo**.  
2. **Cafetal** – visibilidad reducida; **Cacerola**.  
3. **Casa principal** – interiores; **Chancla** (boomerang corto).  
4. **Lago** – movilidad limitada; **Revólver** (precisión).  
5. **Taller** – choke points; **Escopeta** (corto alcance) + **Machete** (final).

Cada área completada otorga **+1 slot de mejora** (vida, daño, regeneración, resistencia al miedo).

---

## 🧰 Armas y Progresión
- **Cuerpo a cuerpo:** **Palo** → **Cacerola** → **Chancla** → **Machete**  
- **A distancia:** **Revólver** → **Escopeta**  
- **Desbloqueo:** al **defender áreas** de la finca.  
- **Munición:**  
  - **Cargador + reserva** (por arma).  
  - **Auto-cambio a melee** al vaciar cargador → **bloqueo 5–8 s** → luego **recargar** si hay balas.  
  - **Drops de balas** en cofres/enemigos; mayor probabilidad en **Establo/Taller**.  
- **Mejoras:** daño, cadencia, control de retroceso, alcance; melee puede ganar **sangrado** o **stun** breve.

---

## 🎯 Objetivos y Condiciones
- **Ganar:** defender todas las áreas y **vencer al Diablo**.  
- **Perder:** Vida = 0 o Integridad del Área objetivo = 0 (en campaña, perder **2 áreas** termina la run).  
- **Rejugabilidad:** rutas de áreas, drops, eventos de locura y clima nocturno aleatorios.

---

## ⌨️ Controles
- **Mover:** WASD  
- **Apuntar:** Mouse  
- **Atacar (contextual):** Click izquierdo  
  - Dispara si hay balas y no estás en bloqueo.  
  - Golpea melee si estás en bloqueo o sin balas.  
- **Melee forzado (opcional):** Click derecho (siempre melee).  
- **Cambiar arma:** *no aplica* (loadout fijo)  
- **Correr:** Shift

---

## ⚙️ Ajustes de Proyecto (Unity)
- **Proyecto 2D ortográfico** (PPU acorde a tu arte).  
- **Sorting Layers:** `Suelo`, `Objetos`, `Jugador`, `Enemigos`, `UI`.  
- **Tilemap** para la finca; colisiones en muros, cercas y árboles.  
- **Cinemachine 2D** para cámara con límites por área.

---

## 🧪 Balance Inicial
> Valores de arranque para tuning (placeholders):

- **Bloqueo melee:** **6 s**.  
- **Revólver:** 6 balas / recarga 1.2 s / daño medio, precisión alta.  
- **Escopeta:** 2–3 balas / recarga 1.6 s / daño alto, corto alcance.  
- **Melee base:** 0.7 s entre golpes; **Machete** mejora rango y daño.  
- **Drops:** munición 18% común; 5% caja grande.  
- **Paletas:** 20% comunes, 5% raras (curan más y reducen más locura).  
- **Escalado por locura:** +5/10/20% daño enemigo en 33/66/100%.  
- **Regeneración:** Vida mínima fuera de combate; la Cordura baja con luz, paletas o amuletos.

---


