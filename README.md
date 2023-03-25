<div align="center" width="100%">
    <img src="https://user-images.githubusercontent.com/30373916/227715640-22e0fa02-8f17-4fbd-a81d-4a010007972a.png" width="150" />
</div>

<div align="center" width="100%">
    <h2>VmChamp</h2>
    <p>Simple and fast creation of VMs on your local machine. Uses KVM, QEMU and libvirt.</p>
    <a target="_blank" href="https://github.com/wubbl0rz/VmChamp"><img src="https://img.shields.io/github/stars/wubbl0rz/VmChamp" /></a>
    <a target="_blank" href="https://github.com/wubbl0rz/VmChamp/releases"><img src="https://img.shields.io/github/v/release/wubbl0rz/VmChamp?display_name=tag" /></a>
    <a target="_blank" href="https://github.com/wubbl0rz/VmChamp/commits/master"><img src="https://img.shields.io/github/last-commit/wubbl0rz/VmChamp" /></a>
</div>

## ✨ Features

- Create throwaway VMs on your local machine and connect via SSH in just a few seconds.
- Fast and easy to use.
- On demand download of latest Debian and Ubuntu cloud images.
- Shell completion.
- Uses KVM, QEMU and libvirt.

## 🚀 Usage

``` bash
VmChamp run mytestvm
# or VmChamp run mytestvm --os debian11 --mem 256MB --disk 4GB
```

<img src="https://user-images.githubusercontent.com/30373916/227714582-0338020d-6d84-4bd8-b3cd-a753cc19e3fa.png" width="700px">

For shell completion put this in your ~.zshrc:

```
source <(VmChamp --completion zsh)
```
