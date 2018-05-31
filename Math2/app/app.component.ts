import { Component } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { WebSocketSubject } from 'rxjs/observable/dom/WebSocketSubject';
import { Game } from "./models/game";
import { Player } from "./models/player";
import { Question } from "./models/question";
import { Answer } from "./models/answer";

@Component({
    selector: 'my-app',
    templateUrl: 'app.component.html',
    moduleId: module.id
})
export class AppComponent {
    name: string = 'User';
    questionText: string = "";
    gameForm: FormGroup;
    question: Question = new Question("", "");
    answer: Answer = new Answer("", "", "");

    public players: Player[] = [];
    private socket$: WebSocketSubject<Game>;

    constructor(private fb: FormBuilder) {
        this.createForm();
    }

    ngOnInit(): void {
    }

    // Create the game signup form
    createForm() {
        this.gameForm = this.fb.group({
            name: ['', Validators.required]
        });
    }

    // Connect to the server and start to receive data
    onSubmit() {
        var data = this.gameForm.get("name").value;
        this.socket$ = WebSocketSubject.create('ws://localhost:3276/api/Game/Register?playerName=' + data);

        this.socket$
            .subscribe(
            (game) => {
                this.name = data;
                this.populatePlayers(game.players);
                this.populateQuestion(game.question);
            },
            (err) => console.error(err),
            () => console.warn('Completed!')
            );
    }

    // Populate playes & points
    populatePlayers(pl: Player[]) {
        this.players = [];
        for (var i = 0; i < pl.length; i++) {
            this.players.push(pl[i]);
        }
    }

    // Populate the questions and display
    populateQuestion(q: Question) {
        if (q !== null) {
            this.question.id = q.id;
            this.questionText = q.questionText;
        }
    }

    submitYes() {
        this.submitAnswer("y");
    }

    submitNo() {
        this.submitAnswer("n");
    }

    // Submit the answer to the server
    submitAnswer(answer: string) {
        try {
            let a = new Answer(this.question.id, answer, this.name);
            var json = JSON.stringify(a);
            console.warn(json);
            this.socket$.socket.send(json);
        } catch (e) {
            alert("Error !");
            console.error(e);
        }
    }
}
