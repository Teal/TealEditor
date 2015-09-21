namespace Teal.CodeEditor {

    /// <summary>
    /// ��ʾһ�������ַ��Ĳ�����
    /// </summary>
    abstract class CharOperation : UndoableOperation {

        /// <summary>
        /// ��ȡ��ǰ�������ַ���
        /// </summary>
        public readonly char value;

        /// <summary>
        /// ��ʼ�� <see cref="CharOperation"/> �����ʵ����
        /// </summary>
        /// <param name="line">��ǰ�������кš�</param>
        /// <param name="column">��ǰ�������кš�</param>
        /// <param name="value">��ǰ�������ַ���</param>
        protected CharOperation(int line, int column, char value)
            : base(line, column) {
            this.value = value;
        }

    }

    /// <summary>
    /// ��ʾ�����ַ��Ĳ�����
    /// </summary>
    sealed class InsertCharOperation : CharOperation {

        /// <summary>
        /// ��ʼ�� <see cref="InsertCharOperation"/> �����ʵ����
        /// </summary>
        /// <param name="line">��ǰ�������кš�</param>
        /// <param name="column">��ǰ�������кš�</param>
        /// <param name="value">��ǰ�������ַ���</param>
        public InsertCharOperation(int line, int column, char value)
            : base(line, column, value) {
        }

        /// <summary>
        /// �жϵ�ǰ�����Ƿ���Ժ�ָ������ͬʱִ�С�
        /// </summary>
        /// <param name="document">Ҫ�������ĵ���</param>
        /// <param name="op">Ҫ�жϵĲ�����</param>
        /// <returns>�������ͬʱִ���򷵻� true�����򷵻� false��</returns>
        public override bool canChain(Document document, UndoableOperation op) {
            var other = op as InsertCharOperation;
            return other != null &&
                line == other.line &&
                column == other.column + 1 &&
                document.syntaxBinding.getCharacterType(value) == document.syntaxBinding.getCharacterType(other.value);
        }

        /// <summary>
        /// ��ָ���༭��ִ�е�ǰ�ĳ���������
        /// </summary>
        /// <param name="document">Ҫ�������ĵ���</param>
        public override void undo(Document document) {
            document.delete(line, column);
        }

        /// <summary>
        /// ��ָ���༭��ִ�е�ǰ�Ļָ�������
        /// </summary>
        /// <param name="document">Ҫ�ָ����ĵ���</param>
        public override void redo(Document document) {
            document.insert(line, column, value);
        }

    }

    /// <summary>
    /// ��ʾɾ���ַ��Ĳ�����
    /// </summary>
    sealed class DeleteCharOperation : CharOperation {

        /// <summary>
        /// ��ʼ�� <see cref="DeleteCharOperation"/> �����ʵ����
        /// </summary>
        /// <param name="line">��ǰ�������кš�</param>
        /// <param name="column">��ǰ�������кš�</param>
        /// <param name="value">��ǰ�������ַ���</param>
        public DeleteCharOperation(int line, int column, char value)
            : base(line, column, value) {

        }

        /// <summary>
        /// ��ָ���༭��ִ�е�ǰ�ĳ���������
        /// </summary>
        /// <param name="document">Ҫ�������ĵ���</param>
        public override void undo(Document document) {
            document.insert(line, column, value);
        }

        /// <summary>
        /// ��ָ���༭��ִ�е�ǰ�Ļָ�������
        /// </summary>
        /// <param name="document">Ҫ�ָ����ĵ���</param>
        public override void redo(Document document) {
            document.delete(line, column);
        }

    }

}