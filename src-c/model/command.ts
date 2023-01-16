export interface iCommand {
  seq: number;
  command: string;
  ns: string;
  d: string;
}

export default class Command implements iCommand {
  seq: number = 1;
  command: string = "data";
  ns: string = "";
  d: string = "";

  constructor(dns: string, b32: string) {
    this.ns = "IIPDNS:" + dns.replace(".i2p", ":i2p_");
    this.d = b32;
  }
}
