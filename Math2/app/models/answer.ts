import { Component } from '@angular/core';

export class Answer {
    constructor(
        public questionId: string,
        public answerText: string,
        public player: string
    ) { }
}