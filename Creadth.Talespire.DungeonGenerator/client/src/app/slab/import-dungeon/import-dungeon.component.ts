import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {SlabService} from '../../shared/web.api.service';
import {FileChangeEvent} from '@angular/compiler-cli/src/perform_watch';
import {ClipboardService} from 'ngx-clipboard';

@Component({
  selector: 'la-import-dungeon',
  templateUrl: './import-dungeon.component.html',
  styleUrls: ['./import-dungeon.component.scss']
})
export class ImportDungeonComponent implements OnInit {
  mainForm: FormGroup;
  slabData: string;
  file: File;

  constructor(private _slabService: SlabService, private _fb: FormBuilder, private _cbService: ClipboardService) {
    this.mainForm = _fb.group({
      "data" : ['', Validators.required],
      "scale" : [3, Validators.required],

    });
  }

  ngOnInit() {
  }

  submit() {
    if (this.mainForm.invalid) return;
    this.processDungeon();
  }



  private processDungeon() {
    this._slabService
    .get({data: this.file, fileName: this.mainForm.value.data}, this.mainForm.value.scale)
    .subscribe(x => {
      this.slabData = 'Slab copied to clipboard';
      this._cbService.copy(x);
    }, e => {
      this.slabData = 'This does not seem like a dungeon data';
    })
  }

  onFileChanged(list: FileList) {
    this.file = list[0];
  }
}
