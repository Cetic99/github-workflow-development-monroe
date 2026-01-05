/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import Spinner from '@ui/components/spinner'
import Backdrop from '@ui/components/backdrop'
import styled from '@emotion/styled'

const Container = styled.div`
  color: var(--primary-dark);
  font-size: 1.25rem;
  font-weight: 500;

  text-align: center;
`

const FullPageLoader = ({ loading = false, message }) => {
  if (!loading) {
    return null
  }

  return (
    <Backdrop open={loading}>
      <Container>
        {loading && <Spinner />}

        {message && <div className="loading-message">{message}</div>}
      </Container>
    </Backdrop>
  )
}

export default FullPageLoader
