# IFT2103A24_TP3_Equipe21

## Description

- Les méthodes d’animation employées pour les agents
- Les méthodes d’animation employées pour l’interface
- La description des effets de particules et de leur contexte d’utilisation
- La description de l’ambiance sonore
- La liste des effets sonores et de leur contexte d’utilisation
- La description de la fonctionnalité optionnelle

**Animation des agents**
- `Squash and Stretch`: Les agents sont animés avec un effet de squash and stretch pour donner un effet de ressort lorsqu'ils se sautent.
- `Anticipation`: Les agents anticipent leur saut en se baissant avant de sauter.
- `Amortissement`: Les agents amortissent leur saut naturellemenetn après le saut.
- `interpolation linéaire`: Les agents se déplacent de manière linéaire entre deux points. (l'initial et la hauteur du saut)
- Déclenchement de l'animation: Les agents déclenchent leur animation de saut lorsqu'ils sautent (appelle de la command `jump` via l'`Invoker`).

**Animation de l’interface**

- Le nombre de bouton de l'interface est dynamique en fonction du nombre de materials disponibles.
- Il y a une animation de fade in et fade out lorsqu'on appui sur un bouton.
- La scale du joueur peut être modifié de 0.5 à 2.0 grace au slider.
- Le volume de la musique, des effets sonores et de l'amiance sonore peut être modifié grace au slider.
- Un son est joué lorsqu'on appui sur le bouton play.
- Pour cliquer sur les boutons, il faut que le curseur soit sur le bas du bouton pour que le clic soit pris en compte.

**Effets de particules**
- `Effet de particules`: Les particules sont utilisées pour simuler un effet de feux _(poussière)_ lorsqu'un agent saute.
- Un système de Pooling est utilisé pour réutiliser les particules et les sauvegarder en cache pour éviter de les recréer à chaque fois.

**Ambiance sonore**
- `Ambiance sonore`: Des musiques d'ambiances sont jouée en boucle pour donner une ambiance relaxante au jeu.
- Des Folleys sont joués aléatoirement.
- Les volumes des musiques et des folleys(ambience) sont ajustables dans le menu.

**Effets sonores**
- `Effets sonores`: Des effets sonores sont joués lorsqu'un agent saute ou que le bouto play est cliqué.
- Pour le bouton play, l'effet est choisi mais pour le saut, l'effet est choisi aléatoirement parmi une liste d'effets.
- Tout les effets sonores/folley peuvent être spacialisé mais pas la musique d'ambiance.
- Chaque track de son peut être activé/désactivé ou changer de volume individuellement.

**Fonctionnalité optionnelle:**
- Musique dynamique (par : Guillaume Papineau) @MasterLaplace
  - Uitilisation du fondu croisé plus du bpm pour les transitions entre les différentes musiques.
  - Changement automatique de la musique si une musique est terminée ou que la scène change.
  - Possibilité d'activer ou de désactiver une track de sons musique/folley/SFX.
  - Possibilité de changer le volume par layer de musique (musique/folley/SFX).
- Personnalisation de l’avatar (par : Martin Boucault) @Martin-Boucault-35
  - Le joueur peut changer la taille de son avatar.
  - Le joueur peut changer la texture de son avatar.

## Usage Direct

Les exécutables sont situés dans le dossier `bin/`.
Pour lancer le jeu, utiliser le `run.bat` ou le `run.sh` fourni à la racine du projet.

```shell
run.bat

# ou

./run.sh
```

le code source est disponible sur github si besoin:
`https://github.com/MasterLaplace/IFT2103A24_TP3_Equipe21.git`
