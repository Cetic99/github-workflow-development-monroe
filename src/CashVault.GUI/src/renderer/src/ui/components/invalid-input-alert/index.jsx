import styled from '@emotion/styled'

const Container = styled.div`
  .triangle {
    width: 0;
    height: 0;
    border-top: 1.5rem solid transparent;
    border-left: 1rem solid var(--bg-dark);
    margin-left: 5rem;
  }

  .error-text {
    background-color: var(--bg-dark);
    border-radius: var(--border-radius-input);
    padding: 1.5rem 3.5rem;
    color: var(--error-dark);
    font-weight: 500;
    font-size: 1.5rem;
  }
`

const InvalidInputAlert = ({ message }) => {
  return (
    <Container>
      <div className="triangle"></div>
      <div className="error-text">{message}</div>
    </Container>
  )
}

export default InvalidInputAlert
