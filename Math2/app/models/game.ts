import { Component } from '@angular/core';
import { Player } from "./player"
import { Question } from "./question"

export class Game {
    constructor(
        public status: number,
        public statusText: string,
        public question: Question,
        public players: Player[]
    ) { }
}