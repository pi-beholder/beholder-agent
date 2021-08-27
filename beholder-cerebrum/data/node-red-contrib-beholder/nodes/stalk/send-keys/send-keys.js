const { stalkName } = require('../common/');

module.exports = function (RED) {
  function SendKeys(config) {
    RED.nodes.createNode(this, config);
    const node = this;
    node.on('input', function (msg) {
      let body = { };
      if (msg.hasOwnProperty("payload")) {
        body = msg.payload;
      } else {
        body = {
          keys: config.keys
        };
      }
      this.send({
        topic: `beholder/stalk/${stalkName}/keyboard/keys`,
        payload: body
      });
    });
  }
  RED.nodes.registerType("send-keys", SendKeys);
}