import { Component, OnInit } from '@angular/core';
import {SlabService} from '../../shared/web.api.service';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ClipboardService} from 'ngx-clipboard';

@Component({
  selector: 'la-j2s',
  templateUrl: './json-2-slab.component.html',
  styleUrls: ['./json-2-slab.component.scss']
})
export class Json2SlabComponent implements OnInit {
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
    .getSlab(JSON.parse(slabData))
    .subscribe(x => {
      this.slabJson = x;
    }, e => {
      this.slabJson = 'Unable to read json data. Either we have a bug or this is not a correct slab json.'
    })
  }
}
