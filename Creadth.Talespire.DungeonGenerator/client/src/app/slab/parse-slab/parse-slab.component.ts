import { Component, OnInit } from '@angular/core';
import {SlabService} from '../../shared/web.api.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ClipboardService} from 'ngx-clipboard';

@Component({
  selector: 'la-parse-slab',
  templateUrl: './parse-slab.component.html',
  styleUrls: ['./parse-slab.component.scss']
})
export class ParseSlabComponent implements OnInit {
  mainForm: FormGroup;
  slabJson: string;

  constructor(private _slabService: SlabService, private _fb: FormBuilder, private _cb: ClipboardService) {
    this.mainForm = _fb.group({
      "slab" : ['', Validators.required]
    });
  }

  ngOnInit() {
  }

  submit() {
    if (this.mainForm.invalid) return;
    this.processSlab(this.mainForm.value.slab);
  }

  paste(event: ClipboardEvent) {
    this.processSlab(event.clipboardData.getData('text'));
  }

  private processSlab(slabData: string) {
    this._slabService
    .getData(slabData)
    .subscribe(x => {
      this.slabJson = JSON.stringify(x);
    }, e => {
      this.slabJson = 'Unable to read slab data. Either we have a bug or this is not a slab.'
    })
  }
}
